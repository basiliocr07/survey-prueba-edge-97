
import { Toaster } from "@/components/ui/toaster";
import { Toaster as Sonner } from "@/components/ui/sonner";
import { TooltipProvider } from "@/components/ui/tooltip";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Index from "./pages/Index";
import CreateSurvey from "./pages/CreateSurvey"; 
import Results from "./pages/Results";
import NotFound from "./pages/NotFound";
import TakeSurvey from "./pages/TakeSurvey";
import Suggestions from "./pages/Suggestions";
import SurveyResponse from "./pages/SurveyResponse";
import CustomerGrowth from "./pages/CustomerGrowth";
import Requirements from "./pages/Requirements";
import Surveys from "./pages/Surveys";
import Dashboard from "./pages/Dashboard";
import About from "./pages/About";
import ClientAccess from "./pages/ClientAccess";

const queryClient = new QueryClient();

const App = () => (
  <QueryClientProvider client={queryClient}>
    <TooltipProvider>
      <Toaster />
      <Sonner />
      <BrowserRouter>
        <div className="flex justify-center min-h-screen">
          <div className="w-[900px] max-w-[900px] h-full bg-white p-1">
            <Routes>
              <Route path="/" element={<Index />} />
              <Route path="/dashboard" element={<Dashboard />} />
              <Route path="/surveys" element={<Surveys />} />
              <Route path="/create" element={<CreateSurvey />} />
              <Route path="/results" element={<Results />} />
              <Route path="/survey/:surveyId" element={<TakeSurvey />} />
              <Route path="/response/:surveyId" element={<SurveyResponse />} />
              <Route path="/suggestions" element={<Suggestions />} />
              <Route path="/customers" element={<CustomerGrowth />} />
              <Route path="/requirements" element={<Requirements />} />
              <Route path="/about" element={<About />} />
              <Route path="/client" element={<ClientAccess />} />
              <Route path="*" element={<NotFound />} />
            </Routes>
          </div>
        </div>
      </BrowserRouter>
    </TooltipProvider>
  </QueryClientProvider>
);

export default App;
