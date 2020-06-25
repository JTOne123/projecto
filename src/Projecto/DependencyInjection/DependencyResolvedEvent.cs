﻿/*
 * Copyright 2017 Wouter Huysentruit
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Projecto.DependencyInjection
{
    /// <summary>
    /// Delegate for the dependency resolved event handler.
    /// </summary>
    /// <param name="eventArgs">Event arguments.</param>
    public delegate void DependencyResolvedEvent(DependencyResolvedEventArgs eventArgs);
}