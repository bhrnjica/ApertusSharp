using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * response of the List Apertus models
 {
    "data": [
        {
            "id": "swiss-ai/apertus-8b-instruct",
            "object": "model",
            "created": 1677610602,
            "owned_by": "openai"
        },
        {
            "id": "swiss-ai/apertus-70b-instruct",
            "object": "model",
            "created": 1677610602,
            "owned_by": "openai"
        },
        {
            "id": "aisingapore/Gemma-SEA-LION-v4-27B-IT",
            "object": "model",
            "created": 1677610602,
            "owned_by": "openai"
        }
    ],
    "object": "list"
}
 */
namespace ApertusSharp.Entities
{
	public class Model
	{
        public required string Id { get; set; }
        public required string Object { get; set; }
        public DateTime Created { get; set; }
        public required string OwnedBy { get; set; }
	}
}
