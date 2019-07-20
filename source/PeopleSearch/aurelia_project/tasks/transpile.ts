import * as fs from "fs";
import * as gulp from "gulp";
import * as changedInPlace from "gulp-changed-in-place";
import * as plumber from "gulp-plumber";
import * as sourcemaps from "gulp-sourcemaps";
import * as notify from "gulp-notify";
import * as rename from "gulp-rename";
import * as ts from "gulp-typescript";
import * as project from "../aurelia.json";
import { CLIOptions, build } from "aurelia-cli";
import * as merge2 from "merge2";

function configureEnvironment() {
    let env = CLIOptions.getEnvironment();

    return gulp
        .src(`aurelia_project/environments/${env}.ts`)
        .pipe(changedInPlace({ firstPass: true }))
        .pipe(rename("environment.ts"))
        .pipe(gulp.dest(project.paths.root));
}

var typescriptCompiler = typescriptCompiler || null;

function buildTypeScript() {
    // Add logic to moment delcartion file to create an ambient module for global import
    fs.readFile("./custom_typings/moment.d.ts", "utf8", (error, data) => {
        let module = data.replace("export = moment;", 'declare module "moment" { export = moment }');

        fs.writeFile("./custom_typings/moment.d.ts", module, null, () => {
            return;
        });
    });

    typescriptCompiler = ts.createProject("tsconfig.json", {
        typescript: require("typescript")
    });

    let dts = gulp.src(project.transpiler.dtsSource);

    let src = gulp
        .src(project.transpiler.source)
        .pipe(changedInPlace({ firstPass: true }));

    return merge2(dts, src)
        .pipe(
            plumber({
                errorHandler: notify.onError("Error: <%= error.message %>")
            })
        )
        .pipe(sourcemaps.init())
        .pipe(typescriptCompiler())
        .pipe(sourcemaps.write({ sourceRoot: "src" }))
        .pipe(build.bundle());
}

export default gulp.series(configureEnvironment, buildTypeScript);
