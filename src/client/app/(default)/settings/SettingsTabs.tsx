"use client";

import { Tab, TabGroup, TabList, TabPanel, TabPanels } from "@headlessui/react";

interface Tab {
  name: string;
  component: React.ReactNode;
}

interface SettingsTabsProps {
  tabs: Tab[];
}

function classNames(...classes: string[]) {
  return classes.filter(Boolean).join(" ");
}

export function SettingsTabs({ tabs }: SettingsTabsProps) {
  return (
    <TabGroup>
      <TabList className="flex space-x-1 rounded-xl bg-indigo-100 p-1">
        {tabs.map((tab) => (
          <Tab
            key={tab.name}
            className={({ selected }) =>
              classNames(
                "w-full rounded-lg py-2.5 text-sm font-medium leading-5",
                "ring-white ring-opacity-60 ring-offset-2 focus:outline-none focus:ring-2",
                selected
                  ? "bg-white text-indigo-700 shadow"
                  : "text-indigo-600 hover:bg-white/[0.12] hover:text-indigo-700"
              )
            }
          >
            {tab.name}
          </Tab>
        ))}
      </TabList>
      <TabPanels className="mt-6">
        {tabs.map((tab) => (
          <TabPanel
            key={tab.name}
            className={classNames(
              "rounded-xl bg-white p-6",
              "ring-white ring-opacity-60 ring-offset-2 focus:outline-none focus:ring-2"
            )}
          >
            {tab.component}
          </TabPanel>
        ))}
      </TabPanels>
    </TabGroup>
  );
}
