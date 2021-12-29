using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sandbox;
using Tools;


namespace TDEditor.Editors
{
	public class TurretComponentWidget : Widget
	{

		public Color SelectionOutline = Color.Parse( "#3ccde7" ) ?? default;

		Label Title;
		public string ComponentName => Title.Text;
		public TurretComponentWidget( Widget parent, Type type, List<PropertyInfo> Properties ) : base( parent )
		{

			BoxLayout lay = new BoxLayout( BoxLayout.Direction.TopToBottom, this );

			Title = new Label( this )
			{
				Text = type.Name.PadLeft( 53 )
			};
			lay.AddSpacingCell( 5 );
			lay.Add( Title );
			lay.AddSpacingCell( 20 );

			var PropertyList = new Widget( this );
			lay.Add( PropertyList, 1 );

			BoxLayout props = new BoxLayout( BoxLayout.Direction.TopToBottom, PropertyList );
			var instance = Activator.CreateInstance( type );

			int commulativey = 0;
			foreach ( var item in Properties.Reverse<PropertyInfo>() )
			{
				Widget PropertyRow = new Widget( PropertyList );
				BoxLayout prop = new BoxLayout( BoxLayout.Direction.LeftToRight, PropertyRow );
				Label proplaybe = new Label( PropertyList )
				{
					Text = item.Name.ToTitleCase(),
					WordWrap = true
				};
				proplaybe.MaximumSize = new( Size.x / 5, 500 );
				Label propValue = new Label( PropertyList )
				{
					Text = (item.GetGetMethod().ReturnType.ToString().Replace( "System.", "" ) + ": " + item.GetValue( instance ).ToString() ?? "null").ToTitleCase(),
					WordWrap = true
				};
				propValue.MaximumSize = new( Size.x / 5, 500 );
				prop.AddSpacingCell( 20 );

				prop.Add( proplaybe );
				prop.Add( propValue );

				props.Add( PropertyRow );
				props.AddSpacingCell( 5 );
				commulativey += proplaybe.Size.y.CeilToInt() + 20;
			}
			props.AddSpacingCell( 10 );
			MinimumSize = new( 0, commulativey );
		}

		bool hovered;

		public override void Update()
		{
			base.Update();
			hovered = IsUnderMouse;
		}

		protected override void OnPaint()
		{
			var rect = new Rect();
			rect.Size = Size;
			Paint.SetPen( Color.White.WithAlpha( 0.4f ), 0 );
			Paint.SetBrush( new Color( 0.2f, 0.2f, 0.2f, 0.9f ) );
			Paint.DrawRect( rect, 10.0f );

			var TitleRect = new Rect( Title.Position.x, Title.Position.y - 2.5f, Title.Size.x, Title.Size.y + 5 );

			Paint.SetPenEmpty();
			Paint.SetBrush( new Color( 0.4f, 0.4f, 0.4f, 0.3f ) );
			Paint.DrawRect( new Rect( TitleRect.Position.x, TitleRect.Position.y, TitleRect.width, TitleRect.height ), 5 );


			foreach ( var item in Children.Last().Children )
			{
				Rect Rect = new Rect( item.Position.x + 10, item.Position.y + item.Parent.Position.y, item.Size.x - 20, item.Size.y );

				Paint.SetPen( new Color( 0.5f, 0.5f, 0.5f, 0.9f ), 1.0f );
				Paint.SetBrush( new Color( 0.1f, 0.1f, 0.1f, 0.9f ) );
				Paint.DrawRect( Rect, 3.0f );

				Paint.SetPenEmpty();
				Paint.SetBrush( new Color( 0.4f, 0.4f, 0.4f, 0.3f ) );
				Paint.DrawRect( new Rect( Rect.Position.x, Rect.Position.y, Rect.width, Rect.height ), 1 );
			}

			if ( hovered )
			{
				Paint.SetPen( SelectionOutline.WithAlpha( 0.5f ), 1.0f );
				Paint.SetBrush( new Color( 1f, 1f, 1f, 0.05f ) );
				Paint.DrawRect( rect, 4.0f );
			}
		}
		protected override void OnDoubleClick( MouseEvent e )
		{
			base.OnDoubleClick( e );
			Log.Error( "Adding Component: " + Title.Text );
		}
	}
}