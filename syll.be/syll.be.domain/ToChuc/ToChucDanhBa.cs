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
    [Table(nameof(ToChucDanhBa), Schema = DbSchemas.Core)]
    [Index(
        nameof(Id),
        IsUnique = false,
        Name = $"IX_{nameof(ToChucDanhBa)}"
    )]
    public class ToChucDanhBa : ISoftDeleted
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdToChuc { get; set; }
        public int IdDanhBa { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool Deleted { get; set; }
        public string? DeletedBy { get; set; }
    }
}
