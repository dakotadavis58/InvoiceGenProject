import { apiClient } from "@/server/api-client";
import { Company } from "@/types/company";
import { CompanySettingsForm } from "./CompanySettingsForm";

export default async function CompanySettings() {
  const company = await apiClient<Company>("/api/company", {
    method: "GET",
  });

  console.log("company", company);

  return (
    <div>
      <CompanySettingsForm initialData={company} />
    </div>
  );
}
