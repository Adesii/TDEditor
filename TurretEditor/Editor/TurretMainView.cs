using System;
using Sandbox;
using Sandbox.Internal;
using Tools;

namespace TDEditor.Editors
{
	public class TurretMainView : Widget
	{
		public static TurretInstance CurrentTurretInstance;
		public TurretMainView( Widget parent ) : base( parent )
		{
			CreateUI();
		}
		BoxLayout MainViewLayout;

		[Event( "turret_editor_reload" )]
		public void CreateUI()
		{
			Clear();
			if ( CurrentTurretInstance == null )
				CurrentTurretInstance = new TurretInstance();
			if ( MainViewLayout == null )
				MainViewLayout = MakeTopToBottom();
			Widget TopRow = new( this );
			MainViewLayout.Add( TopRow, 1 );

			BoxLayout TopRowLayout = TopRow.MakeTopToBottom();

			foreach ( var item in Reflection.GetProperties( CurrentTurretInstance ) )
			{
				if ( item.PropertyType.IsGenericType ) continue;
				Widget RowObject = new( TopRow );
				TopRowLayout.Add( RowObject, 1 );
				BoxLayout Row = RowObject.MakeLeftToRight();
				Label PropertyName = new( "\t\t" + item.Name.ToTitleCase(), RowObject );
				Row.Add( PropertyName, 1 );
				LineEdit lineEdit = new( item.GetValue( CurrentTurretInstance )?.ToString(), RowObject );
				lineEdit.DataBinding = new PropertyBind( CurrentTurretInstance, item );
				lineEdit.PullFromBinding();
				lineEdit.TextEdited += ( sender ) =>
				{
					item.SetValue( CurrentTurretInstance, sender );

				};
				Row.Add( lineEdit, 1 );
			}
			TurretTreeView TurretTreeView = new( this );
			MainViewLayout.Add( TurretTreeView, 3 );

		}

		private void Clear()
		{
			foreach ( var item in Children )
			{
				item?.Destroy();
			}
		}
	}
}
