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
    
    public partial class IslemLog
    {
        public int IslemLogId { get; set; }
        public int IslemLogTipId { get; set; }
        public string KullaniciId { get; set; }
        public string Aciklama { get; set; }
        public System.DateTime Tarih { get; set; }
    
        public virtual IslemLogTip IslemLogTip { get; set; }
        public virtual Kullanici Kullanici { get; set; }
    }
}