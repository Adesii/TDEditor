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



		public TurretEditor( Widget parent ) : base( parent )
		{

			if ( TDWindow.AssemblyDirty )
				GetAllTurretComponents();
			CreateUI();
		}

		private void GetAllTurretComponents()
		{
			var BaseTurretType = TDWindow.LatestTerryDefense.GetType( "TerryDefense.components.turret.BaseTurretComponent" );
			var EntityComponentBlackList = AppDomain.CurrentDomain.GetAssemblies()
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
					if ( !EntityComponentBlackList.Any( ( e ) => e.Name == properties.Name ) )
						propertiesList.Add( properties );
				}

				TurretProperties.Add( item.GetType(), propertiesList );
			}
		}

		RealTimeSince LastFilter;
		private void CreateUI()
		{

			BoxLayout CanvasLayout = new( BoxLayout.Direction.LeftToRight, this );
			Widget LeftColumn = new( this );
			BoxLayout LeftLayout = new( BoxLayout.Direction.TopToBottom, LeftColumn );

			var list = new TurretComponentList( this );


			var SearchBar = new LineEdit( LeftColumn );
			SearchBar.PlaceholderText = "Search";
			SearchBar.MinimumSize = new( 200, 30 );
			SearchBar.TextChanged += ( text ) =>
			{
				if ( LastFilter > 0.05f )
				{
					list.Filter( text );
					LastFilter = 0;

				}

			};
			LeftLayout.Add( SearchBar );
			LeftLayout.Add( list );


			CanvasLayout.Add( LeftColumn, 2 );

			CanvasLayout.Add( new TurretMainView(), 10 );
		}
	}
}