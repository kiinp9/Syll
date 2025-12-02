using Microsoft.EntityFrameworkCore;
using syll.be.shared.Constants.Db;
using syll.be.shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace syll.be.domain.Form
{
    [Table(nameof(FormLoai), Schema = DbSchemas.Core)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(FormLoai)}"
    )]
    public  class FormLoai: ISoftDeleted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TenForm { get; set; } = string.Empty;
        public string MoTa { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsShow { get; set; }
        public DateTime? ThoiGianBatDau { get; set; } 
        public DateTime? ThoiGianKetThuc { get; set; } 
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
