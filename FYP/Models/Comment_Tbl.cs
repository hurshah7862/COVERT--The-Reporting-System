//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FYP.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comment_Tbl
    {
        public int CommentId { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> PostId { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
    
        public virtual Users_Tbl Users_Tbl { get; set; }
        public virtual Posts_Tbl Posts_Tbl { get; set; }
    }
}
