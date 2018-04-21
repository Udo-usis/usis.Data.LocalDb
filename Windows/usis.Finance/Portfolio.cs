//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace usis.Finance
{
    using System;
    using System.Collections.Generic;
    
    public partial class Portfolio
    {
        public Portfolio()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    
        public System.Guid PortfolioId { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> Created { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> Changed { get; set; }
        public string ChangedBy { get; set; }
        public int UpdateCount { get; set; }
        public byte Deleted { get; set; }
    
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
