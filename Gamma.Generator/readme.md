## 🛠 Gamma Generator Usage

To generate code using Gamma, navigate to the `gamma.generator` root directory and run the following command:
```bash
dotnet run -- {ModuleFolder}/{EntityName}.cs
```

### 💡 Example
```bash
dotnet run -- Ast/ProductGroup.cs
```

### ⚠️ Important Notes

The target entity file **must exist** in the Domain layer at the following path before running the generator:
`Domain/Entities/{ModuleFolder}/{EntityName}.cs`

**Expected File Structure:**  
For the example above, the generator expects the file `Domain/Entities/Ast/ProductGroup.cs` to exist and contain a `partial` class:

```bash
public partial class ProductGroup
{
// ...
}
```
