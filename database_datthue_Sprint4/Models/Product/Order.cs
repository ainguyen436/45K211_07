using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using App.Models.Product.Models;
#nullable disable

namespace App.Models.Product 
{
    [Table("Order")]
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("ID")]
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        [Required]
        [StringLength(256)]
        public string UserName { get; set; }
        [StringLength(250)]
        public string UserMessage { get; set; }
        [Required]
        [StringLength(256)]
        public string Email { get; set; }
        [Required]
        [StringLength(256)]
        [Display(Name = "Ngày tạo")]
        public DateTime DateCreated {set; get;}
        public string PaymentStatus { get; set; }
        public bool Status { get; set; }
        [StringLength(400)]
        public string HomeAdress { get; set; }

        // [InverseProperty(nameof(OrderDetail.Order))]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
