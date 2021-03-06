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

namespace NetCore.Profiler.Extension.UI.TimelineCharts
{

    public delegate void ViewPortChangedEventHandler(ITimelineChartModelBase sender);

    public interface ITimelineChartModelBase
    {

        /// <summary>
        /// Maximum timestamp value available
        /// </summary>
        ulong RangeMaxValueMilliseconds { get; }

        /// <summary>
        /// Displayed interval minimal value
        /// </summary>
        ulong ViewPortMinValueMilliseconds { get; }

        /// <summary>
        /// Displayed interval maximum value
        /// </summary>
        ulong ViewPortMaxValueMilliseconds { get; }

        /// <summary>
        /// Regions of the timeline when profiling was paused
        /// </summary>
        List<TimeLineSection> PauseSections { get; }


        ///// <summary>
        ///// Values in the View Port
        ///// </summary>
        //List<T> ViewPortValues { get;}

        /// <summary>
        /// View Port Change Event
        /// </summary>
        event ViewPortChangedEventHandler ViewPortChanged;

    }
}