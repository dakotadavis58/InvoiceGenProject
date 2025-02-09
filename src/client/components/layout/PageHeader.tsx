import { Box, Typography, Breadcrumbs, Button } from "@mui/material";
import Link from "next/link";

interface PageHeaderProps {
  title: string;
  breadcrumbs?: Array<{
    label: string;
    href?: string;
  }>;
  action?: {
    label: string;
    href?: string;
    onClick?: () => void;
    icon?: React.ReactNode;
  };
}

export const PageHeader = ({ title, breadcrumbs, action }: PageHeaderProps) => {
  return (
    <Box sx={{ mb: 4 }}>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 1,
        }}
      >
        <Typography variant="h4" component="h1">
          {title}
        </Typography>
        {action && (
          <Button
            variant="contained"
            color="primary"
            startIcon={action.icon}
            component={action.href ? Link : "button"}
            href={action.href}
            onClick={action.onClick}
          >
            {action.label}
          </Button>
        )}
      </Box>
      {breadcrumbs && (
        <Breadcrumbs aria-label="breadcrumb">
          {breadcrumbs.map((crumb, index) => {
            const isLast = index === breadcrumbs.length - 1;
            return crumb.href && !isLast ? (
              <Link
                key={crumb.label}
                href={crumb.href}
                style={{ color: "inherit", textDecoration: "none" }}
              >
                {crumb.label}
              </Link>
            ) : (
              <Typography key={crumb.label} color="text.primary">
                {crumb.label}
              </Typography>
            );
          })}
        </Breadcrumbs>
      )}
    </Box>
  );
};
