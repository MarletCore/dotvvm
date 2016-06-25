﻿using System;
using System.Collections.Generic;
using DotVVM.Framework.Compilation.ControlTree.Resolved;

namespace DotVVM.Framework.Compilation.ControlTree
{
    public class DataContextStack : IDataContextStack
    {
        public DataContextStack Parent { get; set; }
        public Type DataContextType { get; set; }
        public Type RootControlType { get; set; }
        NamespaceImport[] namespaceImports;
        public NamespaceImport[] NamespaceImports
        {
            get
            {
                return namespaceImports ?? (namespaceImports = Parent?.NamespaceImports);
            }

            set
            {
                namespaceImports = value;
            }
        }

        public DataContextStack(Type type, DataContextStack parent = null)
        {
            Parent = parent;
            DataContextType = type;
            RootControlType = parent?.RootControlType;
        }
        
        public IEnumerable<Type> Enumerable()
        {
            var c = this;
            while (c != null)
            {
                yield return c.DataContextType;
                c = c.Parent;
            }
        }

        public IEnumerable<Type> Parents()
        {
            var c = Parent;
            while(c != null)
            {
                yield return c.DataContextType;
                c = c.Parent;
            }
        }

        ITypeDescriptor IDataContextStack.DataContextType => new ResolvedTypeDescriptor(DataContextType);
        IDataContextStack IDataContextStack.Parent => Parent;
    }
}
