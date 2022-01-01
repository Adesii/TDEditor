using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Sandbox;
using Tools;
using System.ComponentModel;

namespace TDEditor.Editors
{
	public class TurretComponentWidgetEditable : TurretComponentWidget
	{
		public TurretComponentWidgetEditable( TurretComponentList parent, Type type, object Properties, Widget RealParent = null ) : base( parent, type, Properties, RealParent )
		{
			PropertyObject = Properties;
		}

		public override void CreateUI( Type type, List<PropertyInfo> Properties )
		{
			PropertyObject = TurretMainView.CurrentTurretInstance.Components[type];
			BoxLayout lay = MakeTopToBottom();

			Title = new Label( this )
			{
				Text = type.Name,

			};
			lay.AddSpacingCell( 5 );
			lay.Add( Title );
			Title.SetStylesheetFile( "/teststyle.css" );

			lay.AddSpacingCell( 20 );

			Widget AddButton = new( this );
			AddButton.Position = new Vector2( 0, 0 );
			AddButton.Size = new( 50, 50 );

			var PropertyList = new Widget( this );
			lay.Add( PropertyList, 1 );

			BoxLayout props = PropertyList.MakeTopToBottom();


			int commulativey = 0;
			foreach ( var item in Properties.Reverse<PropertyInfo>() )
			{
				Widget PropertyRow = new( PropertyList );
				BoxLayout prop = PropertyRow.MakeLeftToRight();
				Label proplaybe = new( PropertyList )
				{
					Text = item.Name.ToTitleCase(),
					WordWrap = true
				};
				proplaybe.MaximumSize = new( Size.x / 5, 500 );
				Widget propValue = CreateEditor( item );
				//}

				//propValue.TextEdited += ( text ) =>
				//{
				//	
				//};
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
		protected override void OnDoubleClick( MouseEvent e )
		{
			base.OnDoubleClick( e );

			if ( TurretMainView.CurrentTurretInstance?.Components == null )
				TurretMainView.CurrentTurretInstance.Components = new();
			TurretMainView.CurrentTurretInstance?.Components.Remove( ComponentType );

			TurretComponentList.RefreshComponentList();

		}


		public Widget CreateEditor( PropertyInfo property )
		{
			Type type = property.PropertyType;
			if ( type == typeof( bool ) )
			{
				return new CheckBox( this )
				{
					DataBinding = new PropertyBind( PropertyObject, property ),
					State = (bool)property.GetValue( PropertyObject ) ? CheckState.On : CheckState.Off
				};
			}
			else if ( type == typeof( Vector3 ) || type == typeof( Vector2 ) )
			{
				Vector3 vec = new();
				if ( type == typeof( Vector3 ) )
				{
					vec = (Vector3)property.GetValue( PropertyObject );
				}
				else
				{
					vec = (Vector2)property.GetValue( PropertyObject );
				}
				Widget row = new( this );
				var layout = row.MakeLeftToRight();
				{
					var b = new Button( "X", this );
					b.SetSizeMode( SizeMode.CanShrink, SizeMode.Expand );

					var e = new LineEdit( vec.x.ToString(), this );
					layout.Add( b );
					layout.Add( e, 1 );
					e.TextEdited += ( text ) =>
					{
						if ( property.PropertyType == typeof( Vector2 ) )
						{
							Vector2 v = (Vector2)property.GetValue( PropertyObject );
							v.x = text.ToFloat();
							property.SetValue( PropertyObject, v );
						}
						else
						{
							Vector3 v = (Vector3)property.GetValue( PropertyObject );
							v.x = text.ToFloat();
							property.SetValue( PropertyObject, v );
						}
					};
				}

				{
					var b = new Button( "Y", this );
					var e = new LineEdit( vec.y.ToString(), this );
					layout.Add( b );
					layout.Add( e, 1 );
					e.TextEdited += ( text ) =>
					{
						if ( property.PropertyType == typeof( Vector2 ) )
						{
							Vector2 v = (Vector2)property.GetValue( PropertyObject );
							v.y = text.ToFloat();
							property.SetValue( PropertyObject, v );
						}
						else
						{
							Vector3 v = (Vector3)property.GetValue( PropertyObject );
							v.y = text.ToFloat();
							property.SetValue( PropertyObject, v );
						}

					};
				}
				if ( type == typeof( Vector3 ) )
				{
					{
						var b = new Button( "Z", this );
						var e = new LineEdit( vec.z.ToString(), this );
						layout.Add( b );
						layout.Add( e, 1 );
						e.TextEdited += ( text ) =>
						{
							Vector3 v = (Vector3)property.GetValue( PropertyObject );
							v.z = text.ToFloat();
							property.SetValue( PropertyObject, v );
						};
					}
				}


				return row;
			}
			else
			{
				var le = new LineEdit( this )
				{
					DataBinding = new PropertyBind( PropertyObject, property ),
					Text = property.GetValue( PropertyObject )?.ToString()
				};
				le.TextEdited += ( se ) =>
				{
					property.SetValue( PropertyObject, Convert.ChangeType( le.Text, type ) );
				};
				return le;
			}

		}
	}
}
