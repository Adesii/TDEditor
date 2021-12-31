using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;
using Tools;


namespace TDEditor.Editors
{
	public class TurretComponentList : GraphicsView
	{
		TurretEditor parentEditor;

		List<GraphicsWidget> components = new();
		public TurretComponentList( TurretEditor ParentEditor )
		{

			parentEditor = ParentEditor;
			Antialiasing = true;
			TextAntialiasing = true;
			BilinearFiltering = true;
			HorizontalScrollbar = ScrollbarMode.Off;
			SetSizeMode( SizeMode.Default, SizeMode.Expand );
			MinimumSize = new Vector2( 300, 200 );
			MaximumSize = new Vector2( 200000, 200000 );
			Size = new Vector2( 300, 0 );
			SetBackgroundImage( "image/background-grid64.png" );
			UpdatesEnabled = false;
			CreateUI();
			UpdatesEnabled = true;
		}

		Widget LastWidget;

		protected override void OnMouseMove( MouseEvent e )
		{
			base.OnMouseMove( e );
			if ( LastWidget == null || !LastWidget.IsUnderMouse )
			{
				LastWidget?.Update();
				LastWidget = (GetItemAt( ToScene( e.LocalPosition ) ) as GraphicsWidget)?.Widget;

			}
			LastWidget?.Update();
		}
		protected override void OnMouseLeave()
		{
			base.OnMouseLeave();
			if ( LastWidget != null )
			{
				LastWidget.Update();
				LastWidget = null;
			}
		}
		float commulativey = 0;
		public void CreateUI()
		{
			components = new();
			foreach ( var item in TurretEditor.TurretProperties )
			{
				if ( TurretMainView.CurrentTurretInstance?.Components.ContainsKey( item.Key ) ?? false )
				{
					Log.Info( $"{item.Key} is in the turret" );
					continue;
				}
				var comp = new TurretComponentWidget( this, item.Key, Activator.CreateInstance( item.Key ) );
				var idk = new GraphicsWidget( comp )
				{
					Size = new Vector2( 300, 100 )
				};

				idk.HoverEvents = true;

				components.Add( idk );
				Add( idk );
				idk.Position = new Vector2( 0, (commulativey) );

				if ( idk.Size.x + 30 > Size.x )
					MinimumSize = MinimumSize.WithX( idk.Size.x + 30 );

				commulativey += idk.Size.y + 10;
			}
		}


		public void Clear()
		{
			commulativey = 0;
			LastWidget = null;
			foreach ( var item in components )
			{
				if ( item?.IsValid ?? false )
					item?.Destroy();
			}
		}

		public void AddChild( TurretComponentWidget widget )
		{
			var idk = new GraphicsWidget( widget )
			{
				Size = new Vector2( 300, 100 )
			};
			idk.HoverEvents = true;
			Add( idk );
			idk.Position = new Vector2( 0, (commulativey) );

			if ( idk.Size.x + 30 > Size.x )
				MinimumSize = MinimumSize.WithX( idk.Size.x + 30 );

			commulativey += idk.Size.y + 10;

			components.Add( idk );
		}
		[Event.Hotload]
		public void OnHotload()
		{
			if ( TDWindow.AssemblyDirty )
				parentEditor.GetAllTurretComponents();
			Clear();
			if ( CurrentFilter.Length > 0 )
				Filter( CurrentFilter );
			else
				CreateUI();


		}
		public void RefreshComponentList()
		{
			UpdatesEnabled = false;
			Clear();
			CreateUI();
			UpdatesEnabled = true;
		}

		string CurrentFilter = "";

		internal void Filter( string text )
		{
			CurrentFilter = text;
			if ( text.Length == 0 )
			{
				UpdatesEnabled = false;
				Clear();
				CreateUI();
				UpdatesEnabled = true;
			}
			else if ( text.Length > 0 )
			{
				UpdatesEnabled = false;
				Clear();
				components = new();
				foreach ( var item in TurretEditor.TurretProperties.Where( e => e.Key.Name.ToLower().Contains( text.ToLower() ) ) )
				{
					Log.Info( item.Key.Name );
					AddChild( new TurretComponentWidget( this, item.Key, item.Value ) );
				}
				UpdatesEnabled = true;

			}
		}
	}

}
