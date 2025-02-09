import { AuthForm } from "../../../components/auth/AuthForm";
import { signupAction } from "../../actions/auth";

export default function SignupPage() {
  return (
    <AuthForm
      title="Create your account"
      fields={[
        {
          id: "firstName",
          label: "First Name",
          type: "text",
          placeholder: "First Name",
        },
        {
          id: "lastName",
          label: "Last Name",
          type: "text",
          placeholder: "Last Name",
        },
        {
          id: "email",
          label: "Email address",
          type: "email",
          placeholder: "Email address",
        },
        {
          id: "password",
          label: "Password",
          type: "password",
          placeholder: "Password",
        },
      ]}
      submitLabel="Sign up"
      alternateLink={{
        text: "Already have an account?",
        label: "Sign in",
        href: "/login",
      }}
      action={signupAction}
    />
  );
}
