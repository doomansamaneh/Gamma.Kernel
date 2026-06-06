#nullable enable
using System;

namespace {{DomainNamespace}}.Entities.{{Schema}};

public partial class {{Entity}}
{
    public static {{Entity}} Create(
{{CreateParameters}}
    )
    {
        var entity = new {{Entity}}
        {
{{CreateAssignments}}
            RowVersion = 1
        };

        return entity;
    }

    public void Update(
{{UpdateParameters}},
        long rowVersion
    )
    {
{{UpdateAssignments}}
        RowVersion = rowVersion;
    }
}
