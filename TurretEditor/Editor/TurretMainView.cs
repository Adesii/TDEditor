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


		public void CreateUI()
		{
			if ( CurrentTurretInstance == null )
				CurrentTurretInstance = new TurretInstance();

			BoxLayout MainViewLayout = new( BoxLayout.Direction.TopToBottom, this );
			Widget TopRow = new( this );
			MainViewLayout.Add( TopRow, 1 );

			BoxLayout TopRowLayout = new( BoxLayout.Direction.TopToBottom, TopRow );

			foreach ( var item in Reflection.GetProperties( CurrentTurretInstance ) )
			{
				Widget RowObject = new( TopRow );
				TopRowLayout.Add( RowObject, 1 );
				BoxLayout Row = new( BoxLayout.Direction.LeftToRight, RowObject );
				Label PropertyName = new( "\t\t" + item.Name.ToTitleCase(), RowObject );
				Row.Add( PropertyName, 1 );
				LineEdit lineEdit = new( item.GetValue( CurrentTurretInstance )?.ToString(), RowObject );
				lineEdit.DataBinding = new PropertyBind( CurrentTurretInstance, item );
				Row.Add( lineEdit, 1 );
			}
			TurretTreeView TurretTreeView = new( this );
			MainViewLayout.Add( TurretTreeView, 3 );

		}
	}
}
