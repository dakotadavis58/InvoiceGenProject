import { AuthForm } from "../../../components/auth/AuthForm";
import { loginAction } from "../../actions/auth";

export default function LoginPage() {
  return (
    <AuthForm
      title="Sign in to your account"
      fields={[
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
      submitLabel="Sign in"
      alternateLink={{
        text: "Don't have an account?",
        label: "Sign up",
        href: "/signup",
      }}
      action={loginAction}
    />
  );
}
