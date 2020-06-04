using UnityEngine.Assertions;
using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System;

public class Media : object{

	public Region defaultRegion { get; private set; }
	public string src { get; private set; }
	public string id { get; private set; }

	public Media( XmlNode node, Dictionary<string,Region> region, string sourcePath){
		/*#if UNITY_EDITOR
			Assert.IsNotNull(node);
			Assert.IsNotNull(region);
		#endif	*/

		foreach( XmlAttribute nodeAttribute in node.Attributes){

			switch( nodeAttribute.Name ){

				case "id":
					this.id = nodeAttribute.Value;
					break;

				case "src":
					this.src = string.Format("{0}/{1}", sourcePath, nodeAttribute.Value);
					break;

				case "region":
					this.defaultRegion = region[nodeAttribute.Value];
					break;

				default:
					throw new Exception(string.Format("Invalid attribute '{0}'", nodeAttribute.Name));
			}
		}
	}
}