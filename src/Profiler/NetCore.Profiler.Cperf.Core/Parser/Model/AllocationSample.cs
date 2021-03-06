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

namespace NetCore.Profiler.Cperf.Core.Parser.Model
{
    /// <summary>
    /// A <see cref="CperfParser"/> data model class for memory sample ("sam mem") %Core %Profiler trace log records.
    /// Describes memory allocations occurred from a previous memory sample (or from a start of a program - for
    /// a first record of a trace log).
    /// </summary>
    public class AllocationSample
    {
        public ulong InternalId { get; }

        public ulong Ticks { get; }

        public List<AllocationSampleInfo> Allocations { get; } = new List<AllocationSampleInfo>();

        public AllocationSample(ulong internalId, ulong ticks)
        {
            InternalId = internalId;
            Ticks = ticks;
        }
    }
}
