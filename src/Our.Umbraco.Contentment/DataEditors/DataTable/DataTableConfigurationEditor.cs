﻿/* Copyright © 2019 Lee Kelleher, Umbrella Inc and other contributors.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.PropertyEditors;

namespace Our.Umbraco.Contentment.DataEditors
{
    public class DataTableConfigurationEditor : ConfigurationEditor
    {
        public const string DisableSorting = Constants.Conventions.ConfigurationEditors.DisableSorting;
        public const string FieldItems = "fields";
        public const string HideLabel = Constants.Conventions.ConfigurationEditors.HideLabel;
        public const string MaxItems = Constants.Conventions.ConfigurationEditors.MaxItems;
        public const string RestrictWidth = "restrictWidth";
        public const string UsePrevalueEditors = "usePrevalueEditors";

        public DataTableConfigurationEditor()
            : base()
        {
            // TODO: Need to decide how to set the fields, would it be from a DocType, Macro, a POCO, or manually (or offer all options?) [LK:2019-05-15]
            // As a work-in-progress, here is a prototype of the manual approach...

            // NOTE: Excluded these ParameterEditors, as they don't fully support zero-config.
            var exclusions = new[] { "contentpicker", "mediapicker", "entitypicker" };
            var paramEditors = Current.ParameterEditors
                .Select(x => new { name = x.Name, value = x.GetValueEditor().View })
                .Where(x => exclusions.Contains(x.value) == false)
                .OrderBy(x => x.name)
                .ToList();

            var listFields = new[]
            {
                new ConfigurationField
                {
                    Key = "key",
                    Name = "Key",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "name",
                    Name = "Name",
                    View = "textbox"
                },
                new ConfigurationField
                {
                    Key = "view",
                    Name = "Editor",
                    View = IOHelper.ResolveUrl(DropdownListDataEditor.DataEditorViewPath),
                    Config = new Dictionary<string, object>
                    {
                        { DropdownListConfigurationEditor.Items, paramEditors }
                    }
                },
            };

            Fields.Add(
                FieldItems,
                nameof(Fields),
                "Configure the editor fields for the data table.",
                IOHelper.ResolveUrl(DataTableDataEditor.DataEditorViewPath),
                new Dictionary<string, object>()
                {
                    { FieldItems, listFields },
                    { MaxItems, 0 },
                    { DisableSorting, Constants.Values.False },
                    { RestrictWidth, Constants.Values.True },
                    { UsePrevalueEditors, Constants.Values.False }
                });

            Fields.AddMaxItems();

            Fields.Add(
                RestrictWidth,
                "Restrict width?",
                "Select to restrict the width of the data table. This will attempt to make the table to be the same width as the 'Add' button.",
                "boolean");

            Fields.AddHideLabel();

            Fields.AddDisableSorting();
        }

        public override IDictionary<string, object> ToValueEditor(object configuration)
        {
            var config = base.ToValueEditor(configuration);

            config.Add(UsePrevalueEditors, Constants.Values.False);

            return config;
        }
    }
}
