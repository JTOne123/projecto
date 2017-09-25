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

using System.Threading;
using System.Threading.Tasks;
using Projecto.DependencyInjection;

namespace Projecto
{
    /// <summary>
    /// Interface for projections.
    /// </summary>
    /// <typeparam name="TMessageEnvelope">The type of the message envelope used to pass the message including custom information to the handler.</typeparam>
    public interface IProjection<in TMessageEnvelope>
        where TMessageEnvelope : MessageEnvelope
    {
        /// <summary>
        /// Gets the next event sequence number needed by this projection.
        /// </summary>
        /// <param name="lifetimeScope">The connection lifetime scope.</param>
        int GetNextSequenceNumber(IConnectionLifetimeScope lifetimeScope);

        /// <summary>
        /// Passes a message to a matching handler and increments the next sequence number returned by <see cref="GetNextSequenceNumber"/>.
        /// </summary>
        /// <param name="lifetimeScope">The connection lifetime scope.</param>
        /// <param name="messageEnvelope">The message envelope.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task"/> for async execution.</returns>
        Task Handle(IConnectionLifetimeScope lifetimeScope, TMessageEnvelope messageEnvelope, CancellationToken cancellationToken);
    }
}
