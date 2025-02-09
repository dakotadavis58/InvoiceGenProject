import { MenuButton } from "./appbar/MenuButton";
import { NotificationsMenu } from "./appbar/NotificationsMenu";
import { ProfileMenu } from "./appbar/ProfileMenu";
import { ThemeToggle } from "./appbar/ThemeToggle";

interface AppBarProps {
  onSidebarToggle: () => void;
}

export const AppBar = ({ onSidebarToggle }: AppBarProps) => {
  return (
    <header className="fixed top-0 left-0 right-0 h-header z-appBar bg-background-paper dark:bg-dark-background-paper border-b border-gray-200 dark:border-gray-800">
      <div className="flex items-center justify-between px-4 h-full">
        {/* Left section */}
        <div className="flex items-center">
          <MenuButton onToggle={onSidebarToggle} />
          <h1 className="ml-4 text-lg font-semibold text-text-primary dark:text-dark-text-primary">
            Invoice Generator
          </h1>
        </div>

        {/* Right section */}
        <div className="flex items-center space-x-2">
          <ThemeToggle />
          <NotificationsMenu />
          <ProfileMenu />
        </div>
      </div>
    </header>
  );
};
