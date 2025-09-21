using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApertusSharp.Entities
{
	/// <summary>
	/// Represents an AI model available through the Apertus API.
	/// </summary>
	public class Model
	{
		/// <summary>
		/// The unique identifier of the model.
		/// </summary>
        public required string Id { get; set; }
        
        /// <summary>
        /// The object type (typically "model").
        /// </summary>
        public required string Object { get; set; }
        
        /// <summary>
        /// The creation date of the model.
        /// </summary>
        public DateTime Created { get; set; }
        
        /// <summary>
        /// The owner/organization that created the model.
        /// </summary>
        public required string OwnedBy { get; set; }
	}
}
