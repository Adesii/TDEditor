using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sandbox;
using Tools;


namespace TDEditor.Editors
{
	public class TurretEditor : Widget
	{



		public static Dictionary<Type, List<PropertyInfo>> TurretProperties = new();

		public static PropertyInfo[] EntityComponentBlackList;



		public TurretEditor( Widget parent ) : base( parent )
		{

			if ( TDWindow.AssemblyDirty )
				GetAllTurretComponents();
			CreateUI();
		}

		public void GetAllTurretComponents()
		{
			var BaseTurretType = TDWindow.LatestTerryDefense.GetType( "TerryDefense.components.turret.BaseTurretComponent" );
			EntityComponentBlackList = AppDomain.CurrentDomain.GetAssemblies()
														.Where( ( e ) => e.GetName().Name == "Sandbox.Game" )
														.Last()
														.GetType( "Sandbox.EntityComponent" )
														.GetProperties();
			var allInherited = BaseTurretType.Assembly.GetTypes()
					.Where( t => t.IsSubclassOf( BaseTurretType ) && !t.IsAbstract )
					.Select( t => Activator.CreateInstance( t ) );

			TurretProperties = new();

			foreach ( var item in allInherited )
			{
				List<PropertyInfo> propertiesList = new();
				foreach ( var properties in item.GetType().GetProperties() )
				{
					if ( !EntityComponentBlackList.Where( e => e.Name == properties.Name ).Any() )
						propertiesList.Add( properties );
				}

				TurretProperties.Add( item.GetType(), propertiesList );
			}
		}

		private void CreateUI()
		{

			BoxLayout CanvasLayout = MakeLeftToRight();
			Widget LeftColumn = new( this );
			BoxLayout LeftLayout = LeftColumn.MakeTopToBottom();

			var TurretMenu = TDWindow.Instance.MenuBar.AddMenu( "Turret Editor" );

			var list = new TurretComponentList( this );
			TurretMenu.AddOption( "New", "reload", () =>
			{
				TurretMainView.CurrentTurretInstance = new TurretInstance();
				Event.Run( "turret_editor_reload" );
				list.RefreshComponentList();
			} );
			TurretMenu.AddOption( "Save", "reload", () =>
			{
				TurretMainView.CurrentTurretInstance.Save();
				list.RefreshComponentList();
			} );
			TurretMenu.AddOption( "Load", "reload", () =>
			{
				TurretMainView.CurrentTurretInstance.Load();
				list.RefreshComponentList();
			} );



			var SearchBar = new LineEdit( LeftColumn )
			{
				PlaceholderText = "Search",
				MinimumSize = new( 200, 30 )
			};
			SearchBar.TextEdited += ( text ) =>
			{
				list.Filter( text );

			};
			LeftLayout.Add( SearchBar );
			LeftLayout.Add( list );


			CanvasLayout.Add( LeftColumn, 2 );

			CanvasLayout.Add( new TurretMainView( this ), 10 );
		}
	}
}
