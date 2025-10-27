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
    [Table(nameof(FormData), Schema = DbSchemas.Core)]
    [Index(
      nameof(Id),
      IsUnique = false,
      Name = $"IX_{nameof(FormData)}"
    )]
    public class FormData: ISoftDeleted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Data { get; set; } = string.Empty;
        public int IdTruongData { get; set; }
        public int IdDanhBa { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
