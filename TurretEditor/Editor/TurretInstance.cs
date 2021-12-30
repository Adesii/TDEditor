using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace TDEditor.Editors
{
	public class TurretInstance
	{
		public Guid Id;
		public string Name { get; set; } = "TestName";
		public string Description { get; set; }

		public TurretInstance()
		{
			Id = Guid.NewGuid();
		}

		public ObservableDictionary<Type, List<PropertyInfo>> Components = new();
	}
}
