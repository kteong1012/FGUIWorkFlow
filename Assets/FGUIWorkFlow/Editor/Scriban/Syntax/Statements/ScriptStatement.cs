// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using Scriban.Parsing;

namespace Scriban.Syntax
{
    /// <summary>
    /// Base class for all statements.
    /// </summary>
    /// <seealso cref="ScriptNode" />
#if SCRIBAN_PUBLIC
    public
#else
    internal
#endif
    abstract partial class ScriptStatement : ScriptNode
    {
        protected ScriptStatement()
        {
            CanOutput = true;
        }

        public bool CanSkipEvaluation { get; protected set; }


        public bool CanOutput { get; protected set; }
    }
}