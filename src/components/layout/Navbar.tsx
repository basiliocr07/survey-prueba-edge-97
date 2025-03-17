
import { useState, useEffect } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Button } from "@/components/ui/button";
import { BarChart3, Home, Menu, X, MessageSquare, Users, FileText, FileBarChart, LayoutDashboard, Facebook, Linkedin, ChevronDown } from "lucide-react";
import { cn } from '@/lib/utils';
import {
  NavigationMenu,
  NavigationMenuContent,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  NavigationMenuTrigger,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";

export default function Navbar() {
  const [isScrolled, setIsScrolled] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const location = useLocation();

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 10);
    };
    
    window.addEventListener('scroll', handleScroll);
    return () => window.removeEventListener('scroll', handleScroll);
  }, []);

  useEffect(() => {
    setIsMobileMenuOpen(false);
  }, [location.pathname]);

  const toggleMobileMenu = () => setIsMobileMenuOpen(!isMobileMenuOpen);

  const navItems = [
    { path: '/', label: 'Home', icon: <Home className="w-4 h-4 mr-2" /> },
    { path: '/dashboard', label: 'Dashboard', icon: <LayoutDashboard className="w-4 h-4 mr-2" /> },
    { path: '/surveys', label: 'Surveys', icon: <FileBarChart className="w-4 h-4 mr-2" /> },
    { path: '/results', label: 'Analysis', icon: <BarChart3 className="w-4 h-4 mr-2" /> },
    { path: '/suggestions', label: 'Suggestions', icon: <MessageSquare className="w-4 h-4 mr-2" /> },
    { path: '/customers', label: 'Customer Growth', icon: <Users className="w-4 h-4 mr-2" /> },
    { path: '/requirements', label: 'Requirements', icon: <FileText className="w-4 h-4 mr-2" /> },
  ];

  const navMenuItems = [
    {
      label: 'Home',
      icon: <Home className="w-4 h-4 mr-2" />,
      href: '/',
      description: 'Return to the main page'
    },
    {
      label: 'Dashboard',
      icon: <LayoutDashboard className="w-4 h-4 mr-2" />,
      href: '/dashboard',
      description: 'Overview of your latest activity',
      features: [
        { name: 'Recent Surveys', description: 'View your most recent surveys', href: '/dashboard#surveys' },
        { name: 'Latest Suggestions', description: 'Check customer suggestions', href: '/dashboard#suggestions' },
        { name: 'Requirements', description: 'View latest requirements', href: '/dashboard#requirements' },
      ]
    },
    {
      label: 'Surveys',
      icon: <FileBarChart className="w-4 h-4 mr-2" />,
      href: '/surveys',
      description: 'Create and manage surveys',
      features: [
        { name: 'All Surveys', description: 'View all your surveys', href: '/surveys' },
        { name: 'Create Survey', description: 'Build a new survey', href: '/create' },
        { name: 'Survey Responses', description: 'View collected responses', href: '/results' },
      ]
    },
    {
      label: 'Analysis',
      icon: <BarChart3 className="w-4 h-4 mr-2" />,
      href: '/results',
      description: 'Analyze survey results',
      features: [
        { name: 'Data Visualization', description: 'View charts and graphs', href: '/results#charts' },
        { name: 'Response Analytics', description: 'Detailed response analysis', href: '/results#analytics' },
        { name: 'Export Data', description: 'Download survey results', href: '/results#export' },
      ]
    },
    {
      label: 'Suggestions',
      icon: <MessageSquare className="w-4 h-4 mr-2" />,
      href: '/suggestions',
      description: 'Customer feedback and ideas',
      features: [
        { name: 'All Suggestions', description: 'Browse customer suggestions', href: '/suggestions' },
        { name: 'Suggestion Reports', description: 'Analyze suggestion trends', href: '/suggestions#reports' },
      ]
    },
    {
      label: 'Customer Growth',
      icon: <Users className="w-4 h-4 mr-2" />,
      href: '/customers',
      description: 'Manage and grow your customer base',
      features: [
        { name: 'Customer List', description: 'View all customers', href: '/customers' },
        { name: 'Customer Analysis', description: 'Analyze customer growth', href: '/customers#analysis' },
        { name: 'Add Customer', description: 'Add a new customer', href: '/customers#add' },
      ]
    },
    {
      label: 'Requirements',
      icon: <FileText className="w-4 h-4 mr-2" />,
      href: '/requirements',
      description: 'Project requirements and documentation',
      features: [
        { name: 'All Requirements', description: 'View project requirements', href: '/requirements' },
        { name: 'Documentation', description: 'Project documentation', href: '/requirements#docs' },
      ]
    },
    {
      label: 'About',
      href: '/about',
      description: 'Learn more about Execudata',
    }
  ];

  return (
    <header 
      className={cn(
        "fixed top-0 left-0 right-0 z-50 transition-all duration-300 px-6 py-4",
        isScrolled ? "glass shadow-sm" : "bg-transparent"
      )}
    >
      <div className="max-w-7xl mx-auto flex justify-between items-center">
        <Link 
          to="/" 
          className="flex items-center space-x-2 font-bold text-xl tracking-tight transition-opacity duration-200 hover:opacity-80"
        >
          <span className="text-primary">SurveyMaster</span>
        </Link>

        {/* Desktop Navigation with Hover Menus */}
        <NavigationMenu className="hidden md:flex">
          <NavigationMenuList>
            {navMenuItems.map((item) => (
              <NavigationMenuItem key={item.label}>
                {item.features ? (
                  <>
                    <NavigationMenuTrigger className="h-9 px-3">
                      {item.icon}
                      {item.label}
                    </NavigationMenuTrigger>
                    <NavigationMenuContent>
                      <div className="grid gap-3 p-4 w-[400px]">
                        <div className="flex flex-col gap-1">
                          <Link 
                            to={item.href}
                            className="text-sm font-medium leading-none mb-1 group flex items-center"
                          >
                            {item.icon}
                            {item.label}
                          </Link>
                          <p className="text-xs text-muted-foreground mb-2">{item.description}</p>
                          <div className="grid gap-2">
                            {item.features.map((feature) => (
                              <Link
                                key={feature.name}
                                to={feature.href}
                                className="group grid grid-cols-[20px_1fr] gap-1 rounded-md p-2 text-sm items-center hover:bg-accent hover:text-accent-foreground"
                              >
                                <div className="h-1 w-1 rounded-full bg-primary"></div>
                                <div>
                                  <div className="font-medium group-hover:underline">{feature.name}</div>
                                  <div className="text-xs text-muted-foreground">{feature.description}</div>
                                </div>
                              </Link>
                            ))}
                          </div>
                        </div>
                      </div>
                    </NavigationMenuContent>
                  </>
                ) : (
                  <Link to={item.href}>
                    <NavigationMenuLink className={navigationMenuTriggerStyle()}>
                      {item.icon}
                      {item.label}
                    </NavigationMenuLink>
                  </Link>
                )}
              </NavigationMenuItem>
            ))}
          </NavigationMenuList>
        </NavigationMenu>

        {/* Mobile Menu Toggle */}
        <Button 
          variant="ghost" 
          size="icon" 
          onClick={toggleMobileMenu} 
          className="md:hidden"
          aria-label={isMobileMenuOpen ? "Close menu" : "Open menu"}
        >
          {isMobileMenuOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
        </Button>
      </div>

      {/* Mobile Navigation Overlay */}
      <div 
        className={cn(
          "fixed inset-0 bg-background z-40 md:hidden transition-all duration-300 flex flex-col",
          isMobileMenuOpen 
            ? "opacity-100 translate-x-0" 
            : "opacity-0 translate-x-full pointer-events-none"
        )}
      >
        <div className="flex flex-col pt-24 px-6 space-y-4">
          {navItems.map((item) => (
            <Link 
              key={item.path} 
              to={item.path}
              className={cn(
                "px-4 py-3 rounded-md flex items-center text-lg",
                location.pathname === item.path 
                  ? "bg-primary text-primary-foreground" 
                  : "hover:bg-accent hover:text-accent-foreground"
              )}
            >
              {item.icon}
              <span>{item.label}</span>
            </Link>
          ))}
          <Link 
            to="/about"
            className="px-4 py-3 rounded-md flex items-center text-lg hover:bg-accent hover:text-accent-foreground"
          >
            <span>About</span>
          </Link>
        </div>
      </div>
    </header>
  );
}
