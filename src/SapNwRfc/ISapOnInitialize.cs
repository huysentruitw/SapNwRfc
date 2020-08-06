using System;
using System.Collections.Generic;
using System.Text;

namespace SapNwRfc
{
    /// <summary>
    /// Base class for OnInitialize Method.
    /// </summary>
    public interface ISapOnInitialize
    {
        /// <summary>
        /// Method called after class is instantiated and values from SAP assigned to properties.
        /// </summary>
        void OnInitialize();
    }
}
