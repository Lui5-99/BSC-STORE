using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSC.Models.Entities
{
	public class InventoryAudit
	{
		public int AuditId { get; set; }
		public int? ProductId { get; set; }
		public int Change { get; set; }
		public int PrevQuantity { get; set; }
		public int NewQuantity { get; set; }
		public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
		public int? ByUserId { get; set; }
	}
}
