$models = @(
    @{ Name="Policy"; Controller="Policies"; ModelName="Policy"; Plural="Policies" },
    @{ Name="Vehicle"; Controller="Vehicles"; ModelName="Vehicle"; Plural="Vehicles" },
    @{ Name="Claim"; Controller="Claims"; ModelName="Claim"; Plural="Claims" },
    @{ Name="Expense"; Controller="Expenses"; ModelName="Expense"; Plural="Expenses" },
    @{ Name="Billing"; Controller="Billing"; ModelName="Billing"; Plural="Billings" }
)

$basePath = "d:\Vehicle UI\VehicleShield\Views"

foreach ($m in $models) {
    $dir = "$basePath\$($m.Controller)"
    if (!(Test-Path -Path $dir)) {
        New-Item -ItemType Directory -Path $dir | Out-Null
    }

    # Index.cshtml
    $indexContent = @"
@model IEnumerable<VehicleShield.Models.$($m.ModelName)>
@{ ViewData["Title"] = "$($m.Plural)"; }
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px; align-items:flex-start;">
    <div class="auth-card" style="max-width: 1200px; width: 100%;">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>Manage $($m.Plural)</h2>
            <a asp-action="Create" class="btn-auth" style="width: auto; padding: 10px 20px;">Create New</a>
        </div>
        <div class="table-responsive">
            <table class="table table-striped table-hover">
                <thead style="background: linear-gradient(135deg, #6C63FF 0%, #3ECFCF 100%); color: white;">
                    <tr>
                        <!-- Placeholder for headers, normally we reflect over properties -->
                        <th>ID</th>
                        <th>Details</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var item in Model) {
                        <tr>
                            <td>#</td>
                            <td>@item.ToString()</td>
                            <td>
                                <a asp-action="Edit" asp-route-id="@item.GetType().GetProperty("$($m.ModelName)Id")?.GetValue(item)" class="btn btn-sm btn-primary">Edit</a>
                                <a asp-action="Details" asp-route-id="@item.GetType().GetProperty("$($m.ModelName)Id")?.GetValue(item)" class="btn btn-sm btn-info">Details</a>
                                <a asp-action="Delete" asp-route-id="@item.GetType().GetProperty("$($m.ModelName)Id")?.GetValue(item)" class="btn btn-sm btn-danger">Delete</a>
                            </td>
                        </tr>
                    }
                </tbody>
            </table>
        </div>
    </div>
</div>
"@
    Set-Content -Path "$dir\Index.cshtml" -Value $indexContent

    # Create.cshtml
    $createContent = @"
@model VehicleShield.Models.$($m.ModelName)
@{ ViewData["Title"] = "Create $($m.Name)"; }
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 600px; width: 100%;">
        <h2>Create $($m.Name)</h2>
        <form asp-action="Create" method="post" class="auth-form">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <!-- Dynamic fields go here. Just a simple fallback for now -->
            <p class="text-muted">Form generated for $($m.Name). Please update properties manually if needed.</p>
            <div class="form-group">
                <button type="submit" class="btn-auth">Create</button>
            </div>
        </form>
        <a asp-action="Index">Back to List</a>
    </div>
</div>
"@
    Set-Content -Path "$dir\Create.cshtml" -Value $createContent

    # Edit.cshtml
    $editContent = @"
@model VehicleShield.Models.$($m.ModelName)
@{ ViewData["Title"] = "Edit $($m.Name)"; }
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 600px; width: 100%;">
        <h2>Edit $($m.Name)</h2>
        <form asp-action="Edit" method="post" class="auth-form">
            <div asp-validation-summary="ModelOnly" class="text-danger"></div>
            <input type="hidden" asp-for="$($m.ModelName)Id" />
            <!-- Dynamic fields go here -->
            <p class="text-muted">Edit form for $($m.Name).</p>
            <div class="form-group">
                <button type="submit" class="btn-auth">Save</button>
            </div>
        </form>
        <a asp-action="Index">Back to List</a>
    </div>
</div>
"@
    Set-Content -Path "$dir\Edit.cshtml" -Value $editContent

    # Details.cshtml
    $detailsContent = @"
@model VehicleShield.Models.$($m.ModelName)
@{ ViewData["Title"] = "Details $($m.Name)"; }
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 800px; width: 100%;">
        <h2>$($m.Name) Details</h2>
        <div>
            <hr />
            <dl class="row">
                <dt class="col-sm-3">Information</dt>
                <dd class="col-sm-9">@Model.ToString()</dd>
            </dl>
        </div>
        <div>
            <a asp-action="Edit" asp-route-id="@Model?.GetType().GetProperty("$($m.ModelName)Id")?.GetValue(Model)" class="btn btn-primary">Edit</a> |
            <a asp-action="Index">Back to List</a>
        </div>
    </div>
</div>
"@
    Set-Content -Path "$dir\Details.cshtml" -Value $detailsContent

    # Delete.cshtml
    $deleteContent = @"
@model VehicleShield.Models.$($m.ModelName)
@{ ViewData["Title"] = "Delete $($m.Name)"; }
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 800px; width: 100%;">
        <h2>Delete $($m.Name)</h2>
        <h3 class="text-danger">Are you sure you want to delete this?</h3>
        <div>
            <hr />
            <dl class="row">
                <dt class="col-sm-3">Information</dt>
                <dd class="col-sm-9">@Model.ToString()</dd>
            </dl>
            <form asp-action="Delete" method="post">
                <input type="hidden" asp-for="$($m.ModelName)Id" />
                <button type="submit" class="btn btn-danger">Delete</button> |
                <a asp-action="Index">Back to List</a>
            </form>
        </div>
    </div>
</div>
"@
    Set-Content -Path "$dir\Delete.cshtml" -Value $deleteContent
}

# Reports Index
$dir = "$basePath\Reports"
if (!(Test-Path -Path $dir)) { New-Item -ItemType Directory -Path $dir | Out-Null }
$reportContent = @"
@{ ViewData["Title"] = "Reports"; }
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 1200px; width: 100%;">
        <h2>Management Reports</h2>
        <p>This section provides analytical reports and insights into system operations.</p>
        <div class="d-flex" style="gap:20px; flex-wrap:wrap;">
            <a asp-action="MonthlySales" class="btn-auth" style="width:200px;">Monthly Sales</a>
            <a asp-action="VehicleAnalysis" class="btn-auth" style="width:200px;">Vehicle Analysis</a>
            <a asp-action="ClaimsReport" class="btn-auth" style="width:200px;">Claims Report</a>
            <a asp-action="Renewals" class="btn-auth" style="width:200px;">Due Renewals</a>
            <a asp-action="Lapsed" class="btn-auth" style="width:200px;">Lapsed Policies</a>
        </div>
    </div>
</div>
"@
Set-Content -Path "$dir\Index.cshtml" -Value $reportContent

Write-Host "Generated all views successfully!"
