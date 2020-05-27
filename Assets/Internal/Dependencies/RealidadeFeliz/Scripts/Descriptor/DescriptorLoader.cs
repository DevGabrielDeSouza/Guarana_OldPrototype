﻿using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Assertions;
using UnityEngine;
using TMPro;

using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.IO;
using System;

public class DescriptorLoader : object{

	// NCL nodes as Dict<id,class>
	public Dictionary<string,Region> region { get; private set; }
	public Dictionary<string,Media> media { get; private set; }

	public DescriptorLoader(TextAsset docLocation, string TMP_PATH, TextMeshPro debug){

		this.region = new Dictionary<string,Region>();
		this.media = new Dictionary<string,Media>();

		// ################################
		// Primeiro lê-se o NCL

		XmlDocument ncl = new XmlDocument();

debug.text += "\n\tDescLoader - 01";

		try{

			if (docLocation == null)
			{
				debug.text += "\n\tdocLocation eh nulo!!! ";
			}

			ncl.LoadXml(docLocation.text);

debug.text += "\n\tDescLoader - 02";

		}catch( Exception e){

debug.text += "\n\tNCL file could not be read: " + e;
			// TODO - quit application with log
		}


//		Debug.Log("Arquivo NCL foi lido com sucesso!"); // ######## DEGUB ########

		try{

			foreach( XmlNode regionNode in ncl.GetElementsByTagName("region")){

				var aux = new Region(regionNode);
				this.region.Add(aux.id,aux);
			}

debug.text += "\n\tDescLoader - 03";

		}catch( Exception e){

debug.text += "\n\tExceção capturada: " + e;
			// TODO - quit application with log
		}

//		Debug.Log("Li todos as tags 'region'");

		try{

			foreach( XmlNode mediaNode in ncl.GetElementsByTagName("media")){

				var aux = new Media(mediaNode,this.region,TMP_PATH);
				this.media.Add(aux.id,aux);
			}

debug.text += "\n\tDescLoader - 04";

		}catch( Exception e){

debug.text += "\n\tExceção capturada: " + e;
			// TODO - quit application with log
		}

debug.text += "\n\tLi todas as midias?";
//		Debug.Log("Li todos as tags 'media'");
	}
}
