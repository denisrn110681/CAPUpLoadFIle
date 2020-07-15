using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace CAPServer.Models
{
    public class DeliveredFile
    {
        [Key]
        public Guid Id { get; set; }

        public string FileName { get; set; }

        [Required(ErrorMessage = "Data de Entrega e campoo obrigatorio")]
        public DateTime DeliveryDate { get; set; }

        [Required(ErrorMessage = "Descricao do Produto e campo obrigatorio")]
        [MaxLength(50,ErrorMessage="Descricao do Produto deve ter no maximo 50 caracteres")]
        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Quantidade e campo obrigatorio")]
        [Range(1,int.MaxValue , ErrorMessage="Quantiidade deve ser maior que ZERO")]
        public int  Quantity { get; set; }

        [Required(ErrorMessage = "Valor Unitario e campo obrigatorio")]
        public double UnitValue { get; set; }

        public double TotalValue { get; set; }
        
        }
}


