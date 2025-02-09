import CompanySettings from "@/components/settings/CompanySettings";
import ProfileSettings from "@/components/settings/ProfileSettings";
import TemplateSettings from "@/components/settings/TemplateSettings";
import { SettingsTabs } from "./SettingsTabs";

export default async function SettingsPage() {
  const tabs = [
    { name: "Company", component: <CompanySettings /> },
    { name: "Profile", component: <ProfileSettings /> },
    { name: "Templates", component: <TemplateSettings /> },
  ];

  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-2xl font-bold mb-6">Settings</h1>
      <SettingsTabs tabs={tabs} />
    </div>
  );
}
