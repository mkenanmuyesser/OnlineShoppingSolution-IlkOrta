//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CommerceProject.Business.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class SanalPos
    {
        public int SanalPosId { get; set; }
        public int BankaId { get; set; }
        public string Host { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string Currency { get; set; }
        public string ChargeType { get; set; }
        public string PosNumber { get; set; }
        public string Xcip { get; set; }
        public bool DDDAktifMi { get; set; }
        public string DDDStoreKey { get; set; }
        public bool AktifMi { get; set; }
    
        public virtual Banka Banka { get; set; }
    }
}
