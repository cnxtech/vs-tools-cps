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

namespace NetCore.Profiler.Cperf.Core.Parser.Model
{
    /// <summary>
    /// A native code information block item used in <see cref="FunctionInfo"/>.
    /// </summary>
    public class CodeInfo
    {
        public ulong StartAddress { get; }

        public uint Size { get; }

        public CodeInfo(ulong startAddress, uint size)
        {
            StartAddress = startAddress;
            Size = size;
        }
    }
}
