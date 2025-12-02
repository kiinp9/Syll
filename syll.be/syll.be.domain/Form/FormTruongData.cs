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
    [Table(nameof(FormTruongData), Schema = DbSchemas.Core)]
    [Index(
    nameof(Id),
    IsUnique = false,
    Name = $"IX_{nameof(FormTruongData)}"
    )]
    public class FormTruongData : ISoftDeleted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdFormLoai { get; set; }
        public int IdItem { get; set; }
        public string TenTruong { get; set; } = string.Empty;
        //public int Order { get; set; }
        //public bool IsShow { get; set; }
        public int IndexInTemplate { get; set; }
        //public bool IsTruongCustom { get; set; }
        public int BlockTruongNhanBan {  get; set; }
        public bool TruongCanNhap {get; set; }
        public string Type { get; set; } = "string";
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }

    }
}
