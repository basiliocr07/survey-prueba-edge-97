import React from 'react';
import { Link } from 'react-router-dom';
import {
  NavigationMenu,
  NavigationMenuList,
  NavigationMenuItem,
  NavigationMenuContent,
  NavigationMenuTrigger,
  NavigationMenuLink,
} from "@/components/ui/navigation-menu"

export default function Navbar() {
  return (
    <header className="fixed top-0 w-full z-50 bg-white border-b">
      <div className="container flex h-16 items-center">
        <div className="mr-4 flex">
          <Link to="/" className="mr-6 flex items-center space-x-2">
            <img src="/logo.svg" alt="Logo" className="h-8 w-8" />
            <span className="font-bold">SurveyApp</span>
          </Link>
          <NavigationMenu>
            <NavigationMenuList>
              <NavigationMenuItem>
                <NavigationMenuTrigger>Surveys</NavigationMenuTrigger>
                <NavigationMenuContent>
                  <div className="grid w-[400px] gap-3 p-4 md:w-[500px] md:grid-cols-2 lg:w-[600px]">
                    <NavigationMenuLink asChild>
                      <Link to="/surveys" className="block p-3 hover:bg-accent rounded-md">
                        <div className="text-sm font-medium">Surveys</div>
                        <div className="text-sm text-muted-foreground">View all surveys</div>
                      </Link>
                    </NavigationMenuLink>
                    <NavigationMenuLink asChild>
                      <Link to="/create" className="block p-3 hover:bg-accent rounded-md">
                        <div className="text-sm font-medium">Create Survey</div>
                        <div className="text-sm text-muted-foreground">Create a new survey</div>
                      </Link>
                    </NavigationMenuLink>
                    <NavigationMenuLink asChild>
                      <Link to="/results" className="block p-3 hover:bg-accent rounded-md">
                        <div className="text-sm font-medium">Results</div>
                        <div className="text-sm text-muted-foreground">View survey results</div>
                      </Link>
                    </NavigationMenuLink>
                  </div>
                </NavigationMenuContent>
              </NavigationMenuItem>
              <NavigationMenuItem>
                <NavigationMenuTrigger>Feedback</NavigationMenuTrigger>
                <NavigationMenuContent>
                  <div className="grid w-[400px] gap-3 p-4 md:w-[500px] md:grid-cols-2 lg:w-[600px]">
                    <NavigationMenuLink asChild>
                      <Link to="/suggestions" className="block p-3 hover:bg-accent rounded-md">
                        <div className="text-sm font-medium">Suggestions</div>
                        <div className="text-sm text-muted-foreground">Manage customer suggestions</div>
                      </Link>
                    </NavigationMenuLink>
                    <NavigationMenuLink asChild>
                      <Link to="/requirements" className="block p-3 hover:bg-accent rounded-md">
                        <div className="text-sm font-medium">Requirements</div>
                        <div className="text-sm text-muted-foreground">Basic requirements management</div>
                      </Link>
                    </NavigationMenuLink>
                    <NavigationMenuLink asChild>
                      <Link to="/advanced-requirements" className="block p-3 hover:bg-accent rounded-md">
                        <div className="text-sm font-medium">Advanced Requirements</div>
                        <div className="text-sm text-muted-foreground">Advanced requirements dashboard</div>
                      </Link>
                    </NavigationMenuLink>
                  </div>
                </NavigationMenuContent>
              </NavigationMenuItem>
              <NavigationMenuItem>
                <NavigationMenuTrigger>Reports</NavigationMenuTrigger>
                <NavigationMenuContent>
                  <div className="grid w-[400px] gap-3 p-4 md:w-[500px] md:grid-cols-2 lg:w-[600px]">
                    <NavigationMenuLink asChild>
                      <Link to="/dashboard" className="block p-3 hover:bg-accent rounded-md">
                        <div className="text-sm font-medium">Dashboard</div>
                        <div className="text-sm text-muted-foreground">Overview of all metrics</div>
                      </Link>
                    </NavigationMenuLink>
                    <NavigationMenuLink asChild>
                      <Link to="/customers" className="block p-3 hover:bg-accent rounded-md">
                        <div className="text-sm font-medium">Customers</div>
                        <div className="text-sm text-muted-foreground">Customer growth analytics</div>
                      </Link>
                    </NavigationMenuLink>
                  </div>
                </NavigationMenuContent>
              </NavigationMenuItem>
              <NavigationMenuItem>
                <Link to="/about">
                  <NavigationMenuLink className="flex">
                    About
                  </NavigationMenuLink>
                </Link>
              </NavigationMenuItem>
            </NavigationMenuList>
          </NavigationMenu>
        </div>
        <div className="ml-auto flex items-center space-x-4">
          <Link
            to="/client"
            className="text-sm font-medium transition-colors hover:text-primary"
          >
            Client Access
          </Link>
          <Link
            to="/login"
            className="text-sm font-medium text-muted-foreground transition-colors hover:text-primary"
          >
            Login
          </Link>
        </div>
      </div>
    </header>
  );
}
