using UnityEngine.Assertions;
using UnityEngine;

using System.Xml;
using System;

public class Region : object{

	private static readonly float MAXIMUM_HEIGHT = 120.0f;
	private static readonly float MAXIMUM_WIDTH = 120.0f;

	public sealed class Transform{

		public Vector3 position;
		public Vector3 scale;

		public Transform( Region.Transform other){

			Assert.IsNotNull(other);

			this.position = new Vector3(other.position.x,other.position.y,other.position.z);
			this.scale = new Vector3(other.scale.x,other.scale.y,other.scale.z);
		}

		public Transform( Vector3 position, Vector3 scale){

			this.position = position;
			this.scale = scale;
		}
	}

	public string id { get; private set; }
	private Region.Transform _trans;

	// Returns null in case it's a 360º area
	public Region.Transform trans{

		get{ return this._trans == null ? null : new Region.Transform(this._trans); }
		private set{ this._trans = value; }
	}

	public bool IsTotal(){

		return this.trans == null;
	}

	public Region( XmlNode node){

		Assert.IsNotNull(node);
		//Debug.Log("Printando nó: " + node); // ######## Debug ########

		// Local variables
		float azimuthal = float.NaN;
		float polar = float.NaN;

		float height = float.NaN;
		float width = float.NaN;

		int radius = -1;
		int zIndex = -1;

		foreach( XmlAttribute attr in node.Attributes){

			//Debug.Log("Attr: " + ( attr != null ? attr.ToString() : "null" ));  // ######## Debug ########

			switch( attr.Name ){

				case "id":
					this.id = attr.Value;
					break;

				case "azimuthal":
					azimuthal = float.Parse(attr.Value.EndsWith("deg") ? attr.Value.Substring(0,attr.Value.Length-3) : attr.Value);

					if( azimuthal < 0.0f || azimuthal > 360.0f )
						throw new Exception("value of attribute 'azimuthal' is out of range");

					break;

				case "polar":
					polar = float.Parse(attr.Value.EndsWith("deg") ? attr.Value.Substring(0,attr.Value.Length-3) : attr.Value);

					if( polar < 0.0f || polar > 180.0f )
						throw new Exception("value of attribute 'polar' is out of range");

					break;

				case "height":
					height = attr.Value.EndsWith("%") ? MAXIMUM_HEIGHT*float.Parse(attr.Value.Substring(0,attr.Value.Length-1))/100 : float.Parse(attr.Value.EndsWith("deg") ? attr.Value.Substring(0,attr.Value.Length-3) : attr.Value);

					if( height < 1.0f || height > MAXIMUM_HEIGHT )
						throw new Exception("value of attribute 'height' is out of range");

					break;

				case "width":
					width = attr.Value.EndsWith("%") ? MAXIMUM_WIDTH*float.Parse(attr.Value.Substring(0,attr.Value.Length-1))/100 : float.Parse(attr.Value.EndsWith("deg") ? attr.Value.Substring(0,attr.Value.Length-3) : attr.Value);

					if( width < 1.0f || width > MAXIMUM_WIDTH )
						throw new Exception("value of attribute 'width' is out of range");

					break;

				case "radius":
					radius = int.Parse(attr.Value);

					if( radius < 0 || radius > 255 )
						throw new Exception("value of attribute 'radius' is out of range");

					break;

				case "zIndex":
					zIndex = int.Parse(attr.Value);

					if( zIndex < 0 || zIndex > 255 )
						throw new Exception("value of attribute 'zIndex' is out of range");

					break;

				default:
					throw new Exception(string.Format("Invalid attribute '{0}'",attr.Name));
			}
		}

		if( string.IsNullOrEmpty(this.id) )
			throw new Exception("attribute 'id' not found or empty");

		// This is the full region
		if( float.NaN.Equals(azimuthal) && float.NaN.Equals(polar) && float.NaN.Equals(height) && float.NaN.Equals(width) ){

			if( radius != -1 )
				throw new Exception("the attribute 'radius' is defined on a 360º area");

			if( zIndex != -1 )
				throw new Exception("the attribute 'zIndex' is defined on a 360º area");

		// This is a pop up area
		}else if( !float.NaN.Equals(azimuthal) && !float.NaN.Equals(polar) && !float.NaN.Equals(height) && !float.NaN.Equals(width) ){

			// Set default values or not
			radius = radius == -1 ? 1 : radius;
			zIndex = zIndex == -1 ? 0 : zIndex;

			// Makes the zIndex worth
			float r = radius - 0.001f * zIndex;

			// Acquires the screen center position
			Vector3 pos = PolarCartesiano(r,polar,azimuthal);

			// Calculates the scale in such a way the FOV area taken independs on the distance
			Vector3 scl = new Vector3(

				Vector3.Distance(pos,PolarCartesiano(r,polar,azimuthal+width)) * 1.4142f,	// Essa constante só deve funcionar na câmera default do editor
				Vector3.Distance(pos,PolarCartesiano(r,polar+height,azimuthal)) * 1.4142f,	// Essa constante deve ser testada no Óculus
				1
			);

			Debug.Log("Encontrei uma region!\nPosition: " + pos + "\nScale: " + scl);

			this.trans = new Region.Transform(pos,scl);

		// This is an error
		}else{

			throw new Exception("invalid configuration");
		}
	}

	private static Vector3 PolarCartesiano( float radius, float polar, float azimuthal){

		float y = radius * Mathf.Sin((90-polar)*Mathf.PI/180);
		float H = radius * Mathf.Cos((90-polar)*Mathf.PI/180);

		float x = H * Mathf.Sin(azimuthal*Mathf.PI/180);
		float z = H * Mathf.Cos(azimuthal*Mathf.PI/180);

		return new Vector3(x,y,z);
	}
}