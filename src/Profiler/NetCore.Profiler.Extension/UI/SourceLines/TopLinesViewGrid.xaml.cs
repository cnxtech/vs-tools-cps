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
using System.Collections.ObjectModel;
using System.Windows.Controls;
using NetCore.Profiler.Analytics.Model;

namespace NetCore.Profiler.Extension.UI.SourceLines
{
    /// <summary>
    /// Top Lines View Grid
    /// </summary>
    public partial class TopLinesViewGrid : UserControl
    {
        public string Header
        {
            set { GridHeader.Text = value; }
        }

        public string ValueHeader
        {
            set { LinesGrid.Columns[1].Header = value; }
        }

        public StatisticsType FilterType
        {
            get { return ItemAdaptor.StatisticsType; }
            set
            {
                ItemAdaptor.StatisticsType = value;
                UpdateValueHeader(value);
            }
        }

        public bool Inclusive
        {
            get { return ItemAdaptor.Inclusive; }
            set { ItemAdaptor.Inclusive = value; }
        }

        public ObservableCollection<ISourceLineStatistics> Lines { get; set; }

        public SourceLineAdaptor ItemAdaptor
        {
            get;
            private set;
        }


        public TopLinesViewGrid()
        {
            InitializeComponent();
            ItemAdaptor = new SourceLineAdaptor();
            DataContext = this;
        }

        public void SetItemsSource(List<ISourceLineStatistics> lines, IProfilingStatisticsTotals totals)
        {

            ItemAdaptor.Totals = totals;

            Lines = new ObservableCollection<ISourceLineStatistics>(lines);

            LinesGrid.ItemsSource = Lines;
        }

        private void UpdateValueHeader(StatisticsType filterType)
        {
            switch (filterType)
            {
                case StatisticsType.Memory:
                    ValueHeader = "Memory";
                    break;
                case StatisticsType.Time:
                    ValueHeader = "Time";
                    break;
                case StatisticsType.Sample:
                    ValueHeader = "Samples";
                    break;
            }
        }

    }
}
