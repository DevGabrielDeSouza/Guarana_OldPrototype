using UnityEngine.Assertions;
using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System.Xml;

public class Media : object{

	private Region defaultReg = null;
	private string id = null;

	public Media( XmlNode node, Dictionary<string,Region> region){

		Assert.IsNotNull(node);
		Assert.IsNotNull(region);
		Debug.Log("Printando nó: " + node);

		foreach( XmlAttribute attr in node.Attributes){

			Debug.Log("Attr: " + ( attr != null ? attr.ToString() : "null" ));

			if( attr.Name == "id" )
				this.id = attr.Value;
		}
	}

	public string GetID(){

		return id;
	}
}