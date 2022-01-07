using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sandbox;

namespace TDEditor.Editors
{
	[Serializable]
	public class TurretInstance
	{
		public Guid Id;
		public string SaveLocation = "";
		public string Name { get; set; } = "TestName";
		public string Description { get; set; }

		private BaseFileSystem TDAddon => TDWindow.TerryDefenseAddon;

		public TurretInstance()
		{
			Id = Guid.NewGuid();
			Components.CollectionChanged += HandleChanges;
		}

		/// <summary>
		/// 	This is the list of components that make up the turret.
		/// 	<br>Key: Component Type</br>
		/// 	<br>Value: Instance of Component</br>
		/// </summary>
		public ObservableDictionary<Type, object> Components = new();
		[JsonConverter( typeof( DictionaryStringObjectJsonConverter ) )]
		public Dictionary<string, object> SerializableComponents { get; set; } = new();


		private void HandleChanges( object sender, NotifyCollectionChangedEventArgs e )
		{
			SerializableComponents = new();
			foreach ( var item in Components )
			{
				SerializableComponents.Add( item.Key.FullName, item.Value );
			}
		}


		public void Save()
		{
			SaveLocation = $"data/Turrets/{Name.ToLower()}.turret"; //TODO: Find a way to open the File Dialog
			if ( !TDAddon.DirectoryExists( "data/Turrets" ) )
				TDAddon.CreateDirectory( "data/Turrets" );
			var stream = TDAddon.OpenWrite( SaveLocation );
			JsonSerializer.Serialize( stream, this, new JsonSerializerOptions { WriteIndented = true } );
			stream.Close();
		}

		public void Load()
		{
			SaveLocation = $"data/Turrets/{Name.ToLower()}.turret";

			if ( !TDAddon.FileExists( SaveLocation ) )
			{
				Save();
			}
			var turret = TDAddon.ReadJson<TurretInstance>( SaveLocation );
			//Log.Info( $"Loaded Turret: {turret.Name}" );
			//Log.Info( $"Loaded Turret: {turret.Description}" );
			//Log.Info( $"Loaded Turret: {turret.SerializableComponents?.Count}" );
			Components.Clear();
			foreach ( var component in turret.SerializableComponents )
			{
				//Log.Info( $"Loaded Component: {component.Key}" );
				//Log.Info( $"Loaded Component: {component.Value}" );
				//Log.Info( $"Loaded Component: {TDWindow.LatestTerryDefense.GetType( component.Key )}" );
				var instanceComponent = Activator.CreateInstance( TDWindow.LatestTerryDefense.GetType( component.Key ) );
				foreach ( var SavedProperty in component.Value as Dictionary<string, object> )
				{
					foreach ( var property in Reflection.GetProperties( instanceComponent ) )
					{
						if ( property.Name == SavedProperty.Key )
						{
							if ( property.PropertyType.IsEnum )
								property.SetValue( instanceComponent, Enum.Parse( property.PropertyType, SavedProperty.Value.ToString() ) );
							else
								property.SetValue( instanceComponent, SavedProperty.Value );
						}
					}
				}
				Components.Add( instanceComponent.GetType(), instanceComponent );
			}
			Event.Run( "turret_editor_reload" );
		}
	}

}
