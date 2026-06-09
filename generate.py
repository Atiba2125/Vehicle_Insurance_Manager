import os

models = [
    {"Name": "Policy", "Controller": "Policies", "ModelName": "Policy", "Plural": "Policies"},
    {"Name": "Vehicle", "Controller": "Vehicles", "ModelName": "Vehicle", "Plural": "Vehicles"},
    {"Name": "Claim", "Controller": "Claims", "ModelName": "Claim", "Plural": "Claims"},
    {"Name": "Expense", "Controller": "Expenses", "ModelName": "Expense", "Plural": "Expenses"},
    {"Name": "Billing", "Controller": "Billing", "ModelName": "Billing", "Plural": "Billings"}
]

base_path = r"d:\Vehicle UI\VehicleShield\Views"

for m in models:
    dir_path = os.path.join(base_path, m["Controller"])
    os.makedirs(dir_path, exist_ok=True)

    index_content = f"""@model IEnumerable<VehicleShield.Models.{m['ModelName']}>
@{{ ViewData["Title"] = "{m['Plural']}"; }}
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px; align-items:flex-start;">
    <div class="auth-card" style="max-width: 1200px; width: 100%;">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>Manage {m['Plural']}</h2>
            <a asp-action="Create" class="btn-auth" style="width: auto; padding: 10px 20px;">Create New</a>
        </div>
        <div class="table-responsive">
            <table class="table table-striped table-hover">
                <thead style="background: linear-gradient(135deg, #6C63FF 0%, #3ECFCF 100%); color: white;">
                    <tr>
                        <th>ID</th>
                        <th>Details</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    @foreach (var item in Model) {{
                        <tr>
                            <td>#</td>
                            <td>@item.ToString()</td>
                            <td>
                                <a asp-action="Edit" asp-route-id="@item.GetType().GetProperty("{m['ModelName']}Id")?.GetValue(item)" class="btn btn-sm btn-primary" style="margin-right:5px;">Edit</a>
                                <a asp-action="Details" asp-route-id="@item.GetType().GetProperty("{m['ModelName']}Id")?.GetValue(item)" class="btn btn-sm btn-info" style="margin-right:5px;">Details</a>
                                <a asp-action="Delete" asp-route-id="@item.GetType().GetProperty("{m['ModelName']}Id")?.GetValue(item)" class="btn btn-sm btn-danger">Delete</a>
                            </td>
                        </tr>
                    }}
                </tbody>
            </table>
        </div>
    </div>
</div>"""
    with open(os.path.join(dir_path, "Index.cshtml"), "w", encoding="utf-8") as f: f.write(index_content)

    create_content = f"""@model VehicleShield.Models.{m['ModelName']}
@{{ ViewData["Title"] = "Create {m['Name']}"; }}
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 600px; width: 100%;">
        <h2>Create {m['Name']}</h2>
        <form asp-action="Create" method="post" class="auth-form">
            <div asp-validation-summary="ModelOnly" class="text-danger" style="color:#ff6b6b; margin-bottom:15px;"></div>
            <p class="text-muted">Form generated for {m['Name']}. Update properties manually.</p>
            <div class="form-group">
                <button type="submit" class="btn-auth">Create</button>
            </div>
        </form>
        <a asp-action="Index" style="color:#6C63FF; text-decoration:none;">Back to List</a>
    </div>
</div>"""
    with open(os.path.join(dir_path, "Create.cshtml"), "w", encoding="utf-8") as f: f.write(create_content)

    edit_content = f"""@model VehicleShield.Models.{m['ModelName']}
@{{ ViewData["Title"] = "Edit {m['Name']}"; }}
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 600px; width: 100%;">
        <h2>Edit {m['Name']}</h2>
        <form asp-action="Edit" method="post" class="auth-form">
            <div asp-validation-summary="ModelOnly" class="text-danger" style="color:#ff6b6b; margin-bottom:15px;"></div>
            <input type="hidden" asp-for="{m['ModelName']}Id" />
            <p class="text-muted">Edit form for {m['Name']}.</p>
            <div class="form-group">
                <button type="submit" class="btn-auth">Save</button>
            </div>
        </form>
        <a asp-action="Index" style="color:#6C63FF; text-decoration:none;">Back to List</a>
    </div>
</div>"""
    with open(os.path.join(dir_path, "Edit.cshtml"), "w", encoding="utf-8") as f: f.write(edit_content)

    details_content = f"""@model VehicleShield.Models.{m['ModelName']}
@{{ ViewData["Title"] = "Details {m['Name']}"; }}
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 800px; width: 100%;">
        <h2>{m['Name']} Details</h2>
        <div>
            <hr />
            <dl class="row">
                <dt class="col-sm-3">Information</dt>
                <dd class="col-sm-9">@Model.ToString()</dd>
            </dl>
        </div>
        <div style="margin-top:20px;">
            <a asp-action="Edit" asp-route-id="@Model?.GetType().GetProperty("{m['ModelName']}Id")?.GetValue(Model)" class="btn-auth" style="width:100px; display:inline-flex; margin-right:10px;">Edit</a>
            <a asp-action="Index" style="color:#6C63FF; text-decoration:none;">Back to List</a>
        </div>
    </div>
</div>"""
    with open(os.path.join(dir_path, "Details.cshtml"), "w", encoding="utf-8") as f: f.write(details_content)

    delete_content = f"""@model VehicleShield.Models.{m['ModelName']}
@{{ ViewData["Title"] = "Delete {m['Name']}"; }}
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px;">
    <div class="auth-card" style="max-width: 800px; width: 100%;">
        <h2>Delete {m['Name']}</h2>
        <h3 style="color:#ff6b6b; margin-bottom:20px;">Are you sure you want to delete this?</h3>
        <div>
            <hr />
            <dl class="row">
                <dt class="col-sm-3">Information</dt>
                <dd class="col-sm-9">@Model.ToString()</dd>
            </dl>
            <form asp-action="Delete" method="post" style="margin-top:20px;">
                <input type="hidden" asp-for="{m['ModelName']}Id" />
                <button type="submit" class="btn-auth" style="background:#ff6b6b; width:150px; display:inline-flex; margin-right:10px;">Delete</button>
                <a asp-action="Index" style="color:#6C63FF; text-decoration:none;">Back to List</a>
            </form>
        </div>
    </div>
</div>"""
    with open(os.path.join(dir_path, "Delete.cshtml"), "w", encoding="utf-8") as f: f.write(delete_content)

reports_dir = os.path.join(base_path, "Reports")
os.makedirs(reports_dir, exist_ok=True)
report_content = """@{ ViewData["Title"] = "Reports"; }
<div class="auth-layout" style="min-height: calc(100vh - 80px); padding: 40px 20px; align-items:flex-start;">
    <div class="auth-card" style="max-width: 1200px; width: 100%;">
        <h2>Management Reports</h2>
        <p>This section provides analytical reports and insights into system operations.</p>
        <div class="d-flex" style="gap:20px; flex-wrap:wrap; margin-top:30px;">
            <a asp-action="MonthlySales" class="btn-auth" style="width:200px;">Monthly Sales</a>
            <a asp-action="VehicleAnalysis" class="btn-auth" style="width:200px;">Vehicle Analysis</a>
            <a asp-action="ClaimsReport" class="btn-auth" style="width:200px;">Claims Report</a>
            <a asp-action="Renewals" class="btn-auth" style="width:200px;">Due Renewals</a>
            <a asp-action="Lapsed" class="btn-auth" style="width:200px;">Lapsed Policies</a>
        </div>
    </div>
</div>"""
with open(os.path.join(reports_dir, "Index.cshtml"), "w", encoding="utf-8") as f: f.write(report_content)

print("Views generated successfully!")
