using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sandbox;
using Sandbox.Internal;
using Tools;

namespace TDEditor.Editors
{
	public class TurretTreeView : Widget
	{

		List<TurretComponentWidget> ComponentNodes = new();

		BoxLayout layout;
		BoxLayout CanvasLayout;

		ScrollArea Canvas;
		PropertySheet PropertySheet;
		public TurretTreeView( Widget parent = null ) : base( parent )
		{
			layout = MakeTopToBottom();
			Canvas = layout.Add( new ScrollArea( this ), 1 );
			Canvas.HorizontalScrollbarMode = ScrollbarMode.Off;
			Canvas.VerticalScrollbarMode = ScrollbarMode.Auto;

			Canvas.Canvas = new( Canvas );
			Canvas.Canvas.SetSizeMode( SizeMode.Default, SizeMode.CanShrink );
			CanvasLayout = Canvas.Canvas.MakeTopToBottom();

			NewInstance();
			//MinimumSize = new Vector2( 300, 200 );
			//MaximumSize = new Vector2( 200000, 200000 );
			//Size = new Vector2( 300, 0 );
			//SetBackgroundImage( "image/background-grid64.png" );
			TranslucentBackground = true;
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
				//LastWidget = (GetItemAt( ToScene( e.LocalPosition ) ) as GraphicsWidget)?.Widget;

			}
			LastWidget?.Update();
		}
		TurretInstance oldInstance;
		[Event.Hotload, Event( "turret_editor_reload" )]
		public void NewInstance()
		{
			if ( oldInstance != null )
			{
				oldInstance.Components.CollectionChanged -= HandleChanges;
			}
			oldInstance = TurretMainView.CurrentTurretInstance;
			if ( oldInstance != null )
			{
				oldInstance.Components.CollectionChanged += HandleChanges;
			}

			CreateUI();
		}
		public void HandleChanges( object? sender, NotifyCollectionChangedEventArgs e )
		{
			//UpdatesEnabled = false;
			//CreateUI();
			//UpdatesEnabled = true;
		}
		public void CreateUI()
		{
			Clear();

			//ComponentNodes = new();
			if ( TurretMainView.CurrentTurretInstance == null || TurretMainView.CurrentTurretInstance.Components == null )
			{
				Log.Error( "TurretTreeView.CreateUI: CurrentTurretInstance is null" );
				return;
			}
			foreach ( var item in TurretMainView.CurrentTurretInstance?.Components )
			{
				var expander = new ExpandGroup( this );
				var info = DisplayInfo.ForType( item.Key, false );
				expander.Title = info.Name;
				expander.Icon = info.Icon ?? expander.Icon;
				var propsheet = new PropertySheet( Canvas.Canvas );
				foreach ( var props in TurretEditor.TurretProperties[item.Key].Reverse<PropertyInfo>() )
				{
					propsheet.AddProperty( props, item.Value );
				}
				//propsheet.
				//propsheet.Target = item.Value;
				expander.SetWidget( propsheet );
				Button b = new( "", "close", expander );
				b.Size = new Vector2( 20, 20 );

				b.Clicked += () =>
				{
					TurretMainView.CurrentTurretInstance?.Components.Remove( item.Key );
					Event.Run( "turret_editor_reload" );
					Event.Run( "refresh_turret_components" );
				};

				CanvasLayout.Add( expander, 0 );
			}
		}

		public void Clear()
		{
			//foreach ( var item in ComponentNodes )
			//{
			//	item?.Destroy();
			//}
			//if ( !layout?.IsValid ?? true )
			//	layout = Canvas.MakeTopToBottom();
			//layout.Spacing = 20;

			//foreach ( var item in Children )
			//{
			//	item.Destroy();
			//}
			foreach ( var item in Canvas.Canvas.Children )
			{
				item.Destroy();
			}
		}

		public void AddChild( Type type, object properties )
		{
			try
			{
				var comp = new TurretComponentWidgetEditable( null, type, properties );
				comp.SetSizeMode( SizeMode.CanGrow, SizeMode.CanShrink );
				//comp.Title.Text = comp.Title.Text.PadLeft( (int)(Canvas.Size.x * 2.5f) );

				ComponentNodes.Add( comp );
				layout.Add( comp );
			}
			catch ( System.Exception )
			{

			}

		}
	}

}
