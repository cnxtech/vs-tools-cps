﻿/*
 * Copyright 2017 (c) Samsung Electronics Co., Ltd  All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * 	http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Build.Framework.XamlTypes;
using Microsoft.VisualStudio.ProjectSystem;
using Microsoft.VisualStudio.ProjectSystem.Properties;
using Microsoft.VisualStudio.ProjectSystem.Debug;
using System.ComponentModel.Composition;
using System;
using System.Threading.Tasks.Dataflow;
using System.Collections.Immutable;
using Tizen.VisualStudio.Tools.DebugBridge;
using Tizen.VisualStudio;
using Tizen.VisualStudio.ProjectSystem.Debug;

namespace Tizen.VisualStudio.ProjectSystem.VS.Debug
{
    /// <summary>
    /// Custom property page dynamic enum value provider
    /// </summary>
    /*
    TODO: Please refer to the online documentation for current limitations and issues related to IDynamicEnumValuesProvider at https://github.com/Microsoft/VSProjectSystem/blob/master/doc/extensibility/IDynamicEnumValuesProvider.md
    TODO: Follow these additional steps to see the editor in action

    Step1: Insert the following snippet before the closing tag in this file:
    * BuildSystem\Rules\csharp.browseobject.xaml

    <DynamicEnumProperty Name="MyProperty" DisplayName="My Property" Visible="True" Description="Sample property" EnumProvider="DynamicEnumValues1Provider" />

    This will expose a new property "My Property" in the "Properties" pane that uses the editor.

    Step 2: Select a .cs file from a project of your project type (e.g. Program.cs)

    Step 3: In the "Properties" pane, you should see a new property "My Property". Expanding the drop down will show the values generated by the DynamicEnumValues1Generator class.
    */
    [ExportDynamicEnumValuesProvider("DebugProfileEnumValueProvider")]
    [AppliesTo(MyUnconfiguredProject.UniqueCapability)]
    [Export(typeof(IDynamicDebugTargetsGenerator))]
    [ExportMetadata("Name", "DebugProfileEnumValueProvider")]
    public class DebugProfileEnumValueProvider : ProjectValueDataSourceBase<IReadOnlyList<IEnumValue>>, IDynamicEnumValuesProvider, IDynamicDebugTargetsGenerator
    {
        private IReceivableSourceBlock<IProjectVersionedValue<IReadOnlyList<IEnumValue>>> _publicBlock;
        private TransformBlock<string, IProjectVersionedValue<IReadOnlyList<IEnumValue>>> debugProfilesBlock;

        // Represents the link to the launch profiles
        private IDisposable _launchProfileProviderLink;

        // Represents the link to our source provider
        private IDisposable _debugProviderLink;

        private ITizenLaunchSettingsProvider _tizenLaunchSettingsProvider { get; }

        [ImportingConstructor]
        public DebugProfileEnumValueProvider(UnconfiguredProject unconfiguredProject, ITizenLaunchSettingsProvider tizenLaunchSettingProvider)
            : base(unconfiguredProject.Services)
        {
            _tizenLaunchSettingsProvider = tizenLaunchSettingProvider;
        }

        private NamedIdentity _dataSourceKey = new NamedIdentity();
        public override NamedIdentity DataSourceKey
        {
            get { return _dataSourceKey; }
        }


        private int _dataSourceVersion;
        public override IComparable DataSourceVersion
        {
            get { return _dataSourceVersion; }
        }


        public override IReceivableSourceBlock<IProjectVersionedValue<IReadOnlyList<IEnumValue>>> SourceBlock
        {
            get
            {
                EnsureInitialized();
                return _publicBlock;
            }
        }

        // private ILaunchSettingsProvider LaunchSettingProvider { get; }

        ISourceBlock<IProjectVersionedValue<object>> IProjectValueDataSource.SourceBlock => throw new NotImplementedException();

        /// <summary>
        /// Returns an <see cref="IDynamicEnumValuesGenerator"/> instance prepared to generate dynamic enum values
        /// given an (optional) set of options.
        /// </summary>
        /// <param name="options">
        /// A set of options set in XAML that helps to customize the behavior of the
        /// <see cref="IDynamicEnumValuesGenerator "/> instance in some way.
        /// </param>
        /// <returns>
        /// Either a new <see cref="IDynamicEnumValuesGenerator"/> instance
        /// or an existing one, if the existing one can serve responses based on the given <paramref name="options"/>.
        /// </returns>
        public async Task<IDynamicEnumValuesGenerator> GetProviderAsync(IList<NameValuePair> options)
        {
            // TODO: Provide your own implementation
            await Task.Yield();

            return new DebugProfileEnumValueGenerator();
        }

        /*   public IDisposable Join()
           {
               throw new NotImplementedException();
           }
           */
        protected override void Initialize()
        {
            //var debugProfilesBlock = new TransformBlock<ILaunchSettings, IProjectVersionedValue<IReadOnlyList<IEnumValue>>>(
            debugProfilesBlock = new TransformBlock<string, IProjectVersionedValue<IReadOnlyList<IEnumValue>>>(
                update =>
                {
                    // Compute the new enum values from the profile provider
                    var generatedResult = DebugProfileEnumValueGenerator.GetEnumeratorEnumValues().ToImmutableList();
                    _dataSourceVersion++;
                    var dataSources = ImmutableDictionary<NamedIdentity, IComparable>.Empty.Add(DataSourceKey, DataSourceVersion);
                    return new ProjectVersionedValue<IReadOnlyList<IEnumValue>>(generatedResult, dataSources);
                });

            var broadcastBlock = new BroadcastBlock<IProjectVersionedValue<IReadOnlyList<IEnumValue>>>(b => b);

            /* _launchProfileProviderLink = LaunchSettingProvider.SourceBlock.LinkTo(
                 debugProfilesBlock,
                 linkOptions: new DataflowLinkOptions { PropagateCompletion = true });
                 */
            var tizenLaunchSetting = _tizenLaunchSettingsProvider.TizenLaunchSetting;

            _debugProviderLink = debugProfilesBlock.LinkTo(
                broadcastBlock,
                linkOptions: new DataflowLinkOptions { PropagateCompletion = true });

            _publicBlock = broadcastBlock.SafePublicize();
            debugProfilesBlock.Post("InitDebugTargetDeviceList");
            DeviceManager.DebugProfilesBlockList?.Add(debugProfilesBlock);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_launchProfileProviderLink != null)
                {
                    _launchProfileProviderLink.Dispose();
                    _launchProfileProviderLink = null;
                }

                if (_debugProviderLink != null)
                {
                    _debugProviderLink.Dispose();
                    _debugProviderLink = null;
                }

                if (debugProfilesBlock != null && DeviceManager.DebugProfilesBlockList != null)
                {
                    DeviceManager.DebugProfilesBlockList.Remove(debugProfilesBlock);
                }
            }

            base.Dispose(disposing);
        }
    }
}
