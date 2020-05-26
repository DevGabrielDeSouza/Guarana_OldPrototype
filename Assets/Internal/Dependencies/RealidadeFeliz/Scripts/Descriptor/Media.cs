using UnityEngine.Assertions;
using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System;

public class Media : object{

	public Region defaultReg { get; private set; }
	public string src { get; private set; }
	public string id { get; private set; }

	public Media( XmlNode node, Dictionary<string,Region> region, string TMP_PATH){

		Assert.IsNotNull(node);
		Assert.IsNotNull(region);

		foreach( XmlAttribute attr in node.Attributes){

			switch( attr.Name ){

				case "id":
					this.id = attr.Value;
					break;

				case "src":
					this.src = string.Format("{0}/{1}",TMP_PATH,attr.Value);
					break;

				case "region":
					this.defaultReg = region[attr.Value];
					break;

				default:
					throw new Exception(string.Format("Invalid attribute '{0}'",attr.Name));
			}
		}
	}
}