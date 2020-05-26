using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class DescriptorLoader : object{

	// NCL nodes as Dict<id,class>
	public Dictionary<string,Region> region { get; private set; }
	public Dictionary<string,Media> media { get; private set; }

	public DescriptorLoader( string docLocation, string TMP_PATH){

		this.region = new Dictionary<string,Region>();
		this.media = new Dictionary<string,Media>();

		// ################################
		// Primeiro lê-se o NCL

		XmlDocument ncl = new XmlDocument();

		try{

			ncl.Load(docLocation);

		}catch( Exception e){

			Debug.Log("NCL file could not be read: " + e);
			// TODO - quit application with log
		}

//		Debug.Log("Arquivo NCL foi lido com sucesso!"); // ######## DEGUB ########

		try{

			foreach( XmlNode regionNode in ncl.GetElementsByTagName("region")){

				var aux = new Region(regionNode);
				this.region.Add(aux.id,aux);
			}

		}catch( Exception e){

			Debug.Log("Exceção capturada: " + e);
			// TODO - quit application with log
		}

//		Debug.Log("Li todos as tags 'region'");

		try{

			foreach( XmlNode mediaNode in ncl.GetElementsByTagName("media")){

				var aux = new Media(mediaNode,this.region,TMP_PATH);
				this.media.Add(aux.id,aux);
			}

		}catch( Exception e){

			Debug.Log("Exceção capturada: " + e);
			// TODO - quit application with log
		}

//		Debug.Log("Li todos as tags 'media'");
	}
}
