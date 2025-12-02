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

namespace syll.be.domain.ToChuc
{
    [Table(nameof(ToChuc), Schema = DbSchemas.Core)]
    [Index(
     nameof(Id),
     IsUnique = false,
     Name = $"IX_{nameof(ToChuc)}"
    )]
    public class ToChuc: ISoftDeleted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string TenToChuc { get; set; } = String.Empty;
        public string MoTa { get; set; } = String.Empty;
        public int  LoaiToChuc { get; set; }  
        public string? MaSoToChuc { get; set; } = String.Empty;
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
