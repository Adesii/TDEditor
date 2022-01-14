using Sandbox;
using System;
using System.Linq;
using Tools;
using System.Reflection;
using System.Collections.Generic;
using TDEditor.Editors;

namespace TDEditor
{

	[Tool( "Terry Defence Editor", "build", "A Editor to Create many types of things specific to Terry Defence" )]
	public class TDWindow : Window
	{

		public static TDWindow Instance;

		public static Assembly LatestTerryDefense;
		public static BaseFileSystem TerryDefenseAddon = FileSystem.Root.CreateSubSystem( "/addons/terrydefense" );
		public static bool AssemblyDirty = true;

		BoxLayout TabView;

		GraphicsView ActiveTab;

		private Dictionary<string, DockWidget> Docked = new Dictionary<string, DockWidget>();



		public TDWindow()
		{
			if ( Instance != null && Instance.IsValid )
				Close();
			Instance = this;
			Title = "TD Editors";
			Size = new Vector2( 1920, 1080 );

			CreateUI();
			Show();
			MakeMaximized();
		}
		public void CreateUI()
		{
			Clear();

			var menu = MenuBar.AddMenu( "File" );
			menu.AddOption( "Open" );
			menu.AddOption( "Save" );

			menu.AddOption( "Quit" ).Triggered += () => Close();

			var help = MenuBar.AddMenu( "Help" );

			help.AddOption( "Reload Window", "reload", () =>
			{
				CreateUI();
			} );


			var lastAssembly = LatestTerryDefense;
			LatestTerryDefense = GetAssemblyByName( "TerryDefense" );
			if ( lastAssembly != LatestTerryDefense )
			{
				AssemblyDirty = true;
			}
			if ( LatestTerryDefense == null )
			{
				Log.Error( "Load the Gamemode to use the Editor" );
				Close();
				return;
			}


			var TurretButton = AddDock( "Turret", "", DockArea.Top, new TurretEditor( this ) );
			/* var MissionButton = AddDock( "Missions", "", DockArea.Top, new MissionEditor() );

			var TechButton = AddDock( "TechTree", "", DockArea.Top, new TechTreeEditor() ); */


		}

		private DockWidget AddDock( string title, string icon, DockArea area, Widget widget, bool shouldCombine = true )
		{
			DockWidget dockWidget = new DockWidget( title, icon, this );
			dockWidget.Name = title + "DockWidget";
			dockWidget.Widget = widget;
			if ( Docked.Count == 0 || !shouldCombine )
				Dock( dockWidget, area );
			else
				DockInTab( Docked.First().Value, dockWidget );
			Docked[title] = dockWidget;
			return dockWidget;
		}

		public void SetNewTab( GraphicsView view )
		{
			if ( ActiveTab != null )
				ActiveTab.Destroy();
			ActiveTab = view;
			TabView.Add( view );
		}



		Assembly GetAssemblyByName( string name ) //TODO: find a better way to find the assembly
		{
			return AppDomain.CurrentDomain.GetAssemblies().
				LastOrDefault( assembly => assembly.FullName.StartsWith( "Dynamic.local.terrydefense" ) );
		}

		[Sandbox.Event.Hotload]
		public void OnHotload()
		{
			CreateUI();
			Event.Run( "turret_editor_reload" );
		}

		RealTimeSince LastCheck;
		[Event.Frame]
		public void CheckHotload()
		{
			if ( LastCheck > 2 )
			{
				var lastAssembly = LatestTerryDefense;
				LatestTerryDefense = GetAssemblyByName( "TerryDefense" );
				if ( lastAssembly != LatestTerryDefense )
				{
					AssemblyDirty = true;
					Event.Run( "turret_editor_reload" );
					AssemblyDirty = false;
				}
				LastCheck = 0;
			}
		}


	}
}
