import { ProfileSettingsForm } from "./ProfileSettingsForm";
import { ChangePasswordForm } from "./ChangePasswordForm";
import { apiClient } from "@/server/api-client";
import { User } from "@/types/auth";

export default async function ProfileSettings() {
  const userData = await apiClient<User>("/api/users/profile", {
    method: "GET",
  });

  console.log(userData);

  return (
    <div className="space-y-10">
      <div>
        <h2 className="text-lg font-medium text-gray-900">
          Profile Information
        </h2>
        <ProfileSettingsForm initialData={userData} />
      </div>

      <div className="border-t border-gray-200 pt-10">
        <ChangePasswordForm />
      </div>
    </div>
  );
}
